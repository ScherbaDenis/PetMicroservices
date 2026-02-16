# GitHub Copilot Instructions for PetMicroservices

## Project Overview

PetMicroservices is a learning project for microservices architecture built with ASP.NET Core. The repository contains multiple independent microservices following clean architecture principles with separate concerns for domain logic, services, and data access.

## Technology Stack

- **Framework**: .NET 9.0
- **Language**: C# with nullable reference types enabled and implicit usings
- **Database**: SQL Server with Entity Framework Core 9.0.12 (with in-memory option for Answer service)
- **API Protocols**: REST (Comment, Template) and gRPC with JSON transcoding (Answer)
- **Testing**: xUnit with unit and integration tests
- **API Testing**: Postman collections with Newman CLI support
- **CI/CD**: GitHub Actions

## Repository Structure

```
PetMicroservices/
├── BaseWebApplication/     # Base web application template
├── Comment/                # Comment microservice (Legacy Architecture)
│   ├── Comment.Domain/     # Domain models, DTOs, interfaces
│   ├── Comment.Service/    # Business logic services
│   ├── Comment.DataAccess.MsSql/ # EF Core repositories and DbContext
│   ├── WebApiComment/      # ASP.NET Core Web API entry point
│   └── Tests/              # Unit and integration tests
├── Template/               # Template microservice (Legacy Architecture)
│   ├── Template.Domain/
│   ├── Template.Service/
│   ├── Template.DataAccess/
│   ├── WebApiTemplate/
│   └── Tests/
├── Answer/                 # Answer microservice (Modern Clean Architecture)
│   ├── src/
│   │   ├── Answer.Domain/        # Domain entities and common types
│   │   ├── Answer.Application/   # Application interfaces and DTOs
│   │   ├── Answer.Infrastructure/# Data persistence and repositories
│   │   └── Answer.Api/           # gRPC services with REST support
│   ├── tests/
│   │   ├── Answer.Domain.Tests/
│   │   ├── Answer.Infrastructure.Tests/
│   │   └── Answer.Api.IntegrationTests/
│   ├── postman/            # Postman collections for API testing
│   └── Answer.slnx         # Dedicated solution file
└── .github/
    └── workflows/          # CI/CD pipeline definitions
```

## Architecture Patterns

The repository contains microservices using two different architectural approaches:

### Legacy Architecture (Comment, Template services)

**Three-Layer Architecture:**

1. **Domain Layer** (`*.Domain` projects)
   - Contains domain models/entities
   - Defines repository interfaces
   - Defines service interfaces
   - Contains DTOs (Data Transfer Objects)
   - Contains mappers for entity-DTO conversion
   - No dependencies on other layers

2. **Service Layer** (`*.Service` projects)
   - Implements business logic
   - Implements service interfaces from Domain layer
   - Depends on Domain layer only

3. **Data Access Layer** (`*.DataAccess.MsSql` projects)
   - Implements repository interfaces
   - Contains EF Core DbContext
   - Handles database operations
   - Implements Unit of Work pattern

4. **API Layer** (`WebApi*` projects)
   - ASP.NET Core Web API controllers
   - Dependency injection configuration
   - Database seeding for development
   - Entry point (Program.cs)

### Modern Clean Architecture (Answer service)

**Four-Layer Clean Architecture:**

1. **Domain Layer** (`Answer.Domain` project)
   - Contains domain entities
   - Contains common base classes (e.g., BaseEntity)
   - Pure business domain logic
   - No external dependencies

2. **Application Layer** (`Answer.Application` project)
   - Contains application interfaces (e.g., IRepository<T>)
   - Contains DTOs for data transfer
   - Defines contracts for infrastructure
   - Depends only on Domain layer

3. **Infrastructure Layer** (`Answer.Infrastructure` project)
   - Implements application interfaces
   - Contains EF Core DbContext and migrations
   - Implements repositories (both SQL Server and In-Memory)
   - Database configuration and seeding
   - Depends on Application and Domain layers

4. **API Layer** (`Answer.Api` project)
   - gRPC services with JSON transcoding for REST support
   - Dependency injection configuration
   - Entry point (Program.cs)
   - Depends on all other layers

### Key Patterns Used

- **Repository Pattern**: All database access goes through repositories
- **Unit of Work Pattern**: Coordinates multiple repository operations (Legacy services)
- **Generic Repository Pattern**: Single `IRepository<T>` interface for all entities (Answer service)
- **Dependency Injection**: All services registered in DI container
- **Service Layer Pattern**: Business logic separated from controllers/services
- **Strategy Pattern**: Multiple repository implementations (InMemory, SQL Server) for Answer service

## Coding Conventions

### General Guidelines

- Use modern C# features supported by .NET 9.0 with nullable reference types enabled
- Use implicit usings where appropriate
- Follow standard .NET naming conventions (PascalCase for classes/methods, camelCase for parameters)
- Keep controllers thin - business logic belongs in services
- Use async/await for all I/O operations

### Naming Conventions

- Interfaces: Prefix with `I` (e.g., `ICommentService`, `ICommentRepository`)
- DTOs: Suffix with `Dto` when needed to distinguish from entities
- Entities/Models: Use clear, singular nouns (e.g., `Comment`, `Template`)
- Services: Suffix with `Service` (e.g., `CommentService`)
- Repositories: Suffix with `Repository` (e.g., `CommentRepository`)

### API Protocol Conventions

**Legacy Services (Comment, Template) - REST APIs:**
- Use ASP.NET Core Web API controllers
- RESTful routing conventions
- JSON for request/response bodies
- Standard HTTP status codes

**Modern Service (Answer) - gRPC with REST:**
- Primary: gRPC services defined in `.proto` files
- Secondary: REST endpoints via gRPC-JSON transcoding
- Protocol Buffers for gRPC messages
- JSON for REST endpoints (automatically mapped from gRPC)
- gRPC reflection enabled in development for service discovery
- Routes configured using `google.api.http` annotations in proto files

### Entity Framework Conventions

**Legacy Services (Comment, Template):**
- Use `DbContext` classes for database contexts
- Implement `IUnitOfWork` for transaction coordination
- Use fluent API or data annotations for entity configuration
- Enable database seeding in Program.cs using `UseSeeding` and `UseAsyncSeeding`
- Check environment before applying migrations (skip in Testing environment)

**Modern Services (Answer):**
- Use `DbContext` with entity configurations
- Support both SQL Server and in-memory database
- Configure database provider based on `UseInMemoryDatabase` setting
- Use EF Core migrations for schema management
- Automatic database initialization on startup

### Testing Environment

**Legacy Services (Comment, Template):**
- Integration tests use `WebApplicationFactory<Program>`
- Program class must be made public partial for test accessibility: `public partial class Program { }`
- Skip SQL Server configuration in Testing environment using: `if (!builder.Environment.IsEnvironment("Testing"))`

**Modern Services (Answer):**
- Multiple test projects for different concerns:
  - `*.Domain.Tests` - Unit tests for domain entities
  - `*.Infrastructure.Tests` - Unit tests for repositories
  - `*.Api.IntegrationTests` - Integration tests for API endpoints
- Integration tests use `WebApplicationFactory<Program>` with in-memory database
- Comprehensive Postman/Newman test suites in `postman/` directory

## Build, Test, and Development Workflow

### Building the Project

**All services (using main solution):**
```bash
# Restore dependencies
dotnet restore BaseWebApplication/BaseWebApplication.sln

# Build the solution
dotnet build BaseWebApplication/BaseWebApplication.sln

# Build without restore (in CI)
dotnet build --no-restore BaseWebApplication/BaseWebApplication.sln
```

**Answer service (using dedicated solution):**
```bash
# Build Answer service
cd Answer
dotnet build Answer.slnx
```

### Running Tests

**Legacy Services (Comment, Template):**
```bash
# Run specific project tests
dotnet test Comment/Tests/Comment.Tests/Comment.Tests.csproj
dotnet test Template/Tests/Template.Tests/Template.Tests.csproj

# Run with specific options (CI)
dotnet test --no-build --verbosity normal Comment/Tests/Comment.Tests/Comment.Tests.csproj
```

**Modern Service (Answer):**
```bash
# Run all Answer tests
dotnet test Answer/Answer.slnx

# Run specific test projects
dotnet test Answer/tests/Answer.Domain.Tests/Answer.Domain.Tests.csproj
dotnet test Answer/tests/Answer.Infrastructure.Tests/Answer.Infrastructure.Tests.csproj
dotnet test Answer/tests/Answer.Api.IntegrationTests/Answer.Api.IntegrationTests.csproj

# Run with specific options (CI)
dotnet test --no-build --verbosity normal Answer/tests/Answer.Domain.Tests/Answer.Domain.Tests.csproj
```

### Running Services

**Legacy Services:**
```bash
# Run Comment microservice
cd Comment/WebApiComment
dotnet run

# Run Template microservice
cd Template/WebApiTemplate
dotnet run
```

**Modern Service:**
```bash
# Run Answer microservice
cd Answer/src/Answer.Api
dotnet run
# Available at http://localhost:5136
```

### Postman API Testing

Each microservice includes Postman test collections:

**Legacy Services (Comment, Template):**
- **Location**: In the respective `WebApi*` folder
- **Files**:
  - `*.postman_collection.json` - Test collection
  - `*.postman_environment.json` - Environment configuration
  - `POSTMAN_TESTS_README.md` - Documentation

**Modern Service (Answer):**
- **Location**: In the `Answer/postman/` folder
- **Files**:
  - `Answer_API_Tests.postman_collection.json` - Test collection (21+ tests)
  - `Answer_API_Development.postman_environment.json` - Environment configuration
  - `README.md` - Comprehensive documentation

**Running Postman tests with Newman:**

```bash
# Comment microservice
newman run Comment/WebApiComment/WebApiComment.postman_collection.json \
  -e Comment/WebApiComment/WebApiComment.postman_environment.json

# Template microservice  
newman run Template/WebApiTemplate/WebApiTemplate.postman_collection.json \
  -e Template/WebApiTemplate/WebApiTemplate.postman_environment.json

# Answer microservice
cd Answer/postman
newman run Answer_API_Tests.postman_collection.json \
  -e Answer_API_Development.postman_environment.json
```

## CI/CD Pipeline

### GitHub Actions Workflows

1. **Main.yml** - Runs on pull requests to main
   - Builds the solution
   - Runs all tests (Template, Comment, and Answer)
   - Uses .NET 9.0.x

2. **Comment.yml** - Runs on push when Comment folder changes
   - Builds and tests Comment microservice
   - Uses .NET 9.0.x

3. **Template.yml** - Runs on push when Template folder changes
   - Builds and tests Template microservice
   - Uses .NET 9.0.x

4. **Answer.yml** - Runs on push when Answer folder changes
   - Builds and tests Answer microservice (all three test projects)
   - Uses .NET 9.0.x

### Workflow Steps

**Legacy Services (Comment, Template):**
1. Checkout code
2. Setup .NET 9.0.x
3. Restore dependencies from BaseWebApplication.sln
4. Build with `--no-restore`
5. Test with `--no-build --verbosity normal`

**Modern Service (Answer):**
1. Checkout code
2. Setup .NET 9.0.x
3. Restore dependencies from BaseWebApplication.sln
4. Build with `--no-restore`
5. Test all three test projects:
   - Answer.Domain.Tests
   - Answer.Infrastructure.Tests
   - Answer.Api.IntegrationTests

## Database Configuration

**Legacy Services (Comment, Template):**
- **Development**: Uses SQL Server with automatic migrations and data seeding
- **Testing**: Skips SQL Server configuration (uses in-memory or test doubles)
- **Connection String**: Retrieved from `appsettings.json` via `GetConnectionString("DefaultConnection")`

**Modern Service (Answer):**
- **Development**: Supports both in-memory and SQL Server databases
  - Default: In-memory database (no setup required)
  - SQL Server: Set `UseInMemoryDatabase: false` in configuration
- **Testing**: Uses in-memory database for all tests
- **Connection String**: Retrieved from `appsettings.json` via `GetConnectionString("DefaultConnection")`
- **Configuration**: Set via `UseInMemoryDatabase` configuration key

### Database Seeding

**Legacy Services:**
- Only seeds data in Development environment
- Uses `UseSeeding` for synchronous seeding
- Uses `UseAsyncSeeding` for running migrations
- Always checks if data exists before seeding to avoid duplicates

**Modern Service (Answer):**
- Automatic database initialization on startup
- Seeds sample data for in-memory database
- Uses EF Core migrations for SQL Server schema management

## Adding New Features

### Choosing an Architecture Pattern

When creating a new microservice, choose the appropriate architecture:

**Use Legacy Architecture (Service Layer Pattern) when:**
- Building a simple CRUD service with straightforward business logic
- Following the existing Comment/Template pattern
- Traditional REST API is sufficient
- Team is more familiar with this pattern

**Use Modern Clean Architecture (Answer Pattern) when:**
- Building a service with complex domain logic
- Need gRPC support with REST as secondary
- Want multiple storage options (in-memory, SQL Server)
- Following Microsoft's recommended Clean Architecture
- Building a service that may evolve significantly

### Creating a New Microservice

**Legacy Architecture Pattern (Comment/Template style):**
1. Create `{Name}.Domain` project with models, DTOs, interfaces, and mappers
2. Create `{Name}.Service` project with service implementations
3. Create `{Name}.DataAccess.MsSql` project with repositories and DbContext
4. Create `WebApi{Name}` project with controllers and Program.cs
5. Create `Tests/{Name}.Tests` project with unit/integration tests
6. Add to BaseWebApplication.sln
7. Create GitHub Actions workflow in `.github/workflows/{Name}.yml`
8. Add Postman collection in `WebApi{Name}/` folder

**Modern Clean Architecture Pattern (Answer style):**
1. Create folder structure:
   ```
   {Name}/
   ├── src/
   │   ├── {Name}.Domain/        # Entities, enums, base classes
   │   ├── {Name}.Application/   # Interfaces, DTOs
   │   ├── {Name}.Infrastructure/# Repositories, DbContext
   │   └── {Name}.Api/           # gRPC services, Program.cs
   ├── tests/
   │   ├── {Name}.Domain.Tests/
   │   ├── {Name}.Infrastructure.Tests/
   │   └── {Name}.Api.IntegrationTests/
   ├── postman/                  # Postman collections
   └── {Name}.slnx              # Solution file
   ```
2. Implement layers following dependency rules (Domain ← Application ← Infrastructure ← Api)
3. Create multiple test projects for different concerns
4. Add to BaseWebApplication.sln
5. Create GitHub Actions workflow in `.github/workflows/{Name}.yml` with .NET 9.0.x
6. Add comprehensive Postman collection in `postman/` folder
7. Create README.md with architecture documentation

### Adding New Endpoints

**Legacy Services (Comment, Template):**
1. Define service interface in Domain layer
2. Implement service in Service layer
3. Create controller in API layer
4. Add integration tests
5. Update Postman collection

**Modern Service (Answer):**
1. Define entity in Domain layer (if needed)
2. Add DTO in Application layer
3. Implement gRPC service in Api layer
4. Configure JSON transcoding routes
5. Add unit tests, infrastructure tests, and integration tests
6. Update Postman collection

### Adding New Entities

**Legacy Services (Comment, Template):**
1. Create entity class in `{Service}.Domain/Models/`
2. Create corresponding DTO in `{Service}.Domain/DTOs/`
3. Create mapper in `{Service}.Domain/Mappers/`
4. Define repository interface in `{Service}.Domain/Repositories/`
5. Implement repository in `{Service}.DataAccess.MsSql/Repositories/`
6. Add DbSet to DbContext
7. Create and apply EF Core migration

**Modern Service (Answer):**
1. Create entity class in `{Service}.Domain/Entities/`
2. Create corresponding DTO in `{Service}.Application/DTOs/`
3. Update `IRepository<T>` usage (no need for specific repository)
4. Add DbSet to DbContext in Infrastructure layer
5. Configure entity in DbContext if needed
6. Create and apply EF Core migration
7. Add entity tests in Domain.Tests
8. Add repository tests in Infrastructure.Tests

## Pull Request Guidelines

- All PRs to main branch trigger the Test workflow
- PRs must pass all automated tests
- Keep changes focused on a single microservice when possible
- Include tests for new features
- Update Postman collections if API changes

## Common Issues and Solutions

### Testing Environment Configuration

Always wrap SQL Server configuration with environment check:
```csharp
if (!builder.Environment.IsEnvironment("Testing"))
{
    // SQL Server configuration here
}
```

### Making Program.cs Testable

Add at the end of Program.cs:
```csharp
public partial class Program { }
```

### Unique IDs Across Services

Use different GUID prefixes for different services to avoid conflicts:
- Comment service: `10000000-0000-0000-0000-000000000001`, `10000000-0000-0000-0000-000000000002`, etc.
- Template service: `20000000-xxxx-xxxx-xxxx-xxxxxxxxxxxx`
- Answer service: Uses auto-generated GUIDs (no seeding with fixed IDs for in-memory mode)

## Project Goals

This is a learning project focused on:
- Microservices architecture
- Clean architecture principles (both traditional and modern approaches)
- Entity Framework Core
- Repository and Unit of Work patterns
- gRPC with REST support (Answer service)
- Multiple storage strategies (SQL Server, In-Memory)
- Comprehensive testing strategies
- Integration testing
- CI/CD with GitHub Actions
