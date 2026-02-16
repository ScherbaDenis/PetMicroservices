# GitHub Copilot Instructions for PetMicroservices

## Project Overview

PetMicroservices is a learning project for microservices architecture built with ASP.NET Core. The repository contains multiple independent microservices following clean architecture principles with separate concerns for domain logic, services, and data access.

## Technology Stack

- **Framework**: .NET 8.0
- **Language**: C# with nullable reference types enabled and implicit usings
- **Database**: SQL Server with Entity Framework Core 9.0.12
- **Testing**: xUnit with integration tests
- **API Testing**: Postman collections with Newman CLI support
- **CI/CD**: GitHub Actions

## Repository Structure

```
PetMicroservices/
├── BaseWebApplication/     # Base web application template
├── Comment/                # Comment microservice
│   ├── Comment.Domain/     # Domain models, DTOs, interfaces
│   ├── Comment.Service/    # Business logic services
│   ├── Comment.DataAccess.MsSql/ # EF Core repositories and DbContext
│   ├── WebApiComment/      # ASP.NET Core Web API entry point
│   └── Tests/              # Unit and integration tests
├── Template/               # Template microservice (follows same structure)
│   ├── Template.Domain/
│   ├── Template.Service/
│   ├── Template.DataAccess/
│   ├── WebApiTemplate/
│   └── Tests/
└── .github/
    └── workflows/          # CI/CD pipeline definitions
```

## Architecture Patterns

### Clean Architecture Layers

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

### Key Patterns Used

- **Repository Pattern**: All database access goes through repositories
- **Unit of Work Pattern**: Coordinates multiple repository operations
- **Dependency Injection**: All services registered in DI container
- **Service Layer Pattern**: Business logic separated from controllers

## Coding Conventions

### General Guidelines

- Use modern C# features supported by .NET 8.0 with nullable reference types enabled
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

### Entity Framework Conventions

- Use `DbContext` classes for database contexts
- Implement `IUnitOfWork` for transaction coordination
- Use fluent API or data annotations for entity configuration
- Enable database seeding in Program.cs using `UseSeeding` and `UseAsyncSeeding`
- Check environment before applying migrations (skip in Testing environment)

### Testing Environment

- Integration tests use `WebApplicationFactory<Program>`
- Program class must be made public partial for test accessibility: `public partial class Program { }`
- Skip SQL Server configuration in Testing environment using: `if (!builder.Environment.IsEnvironment("Testing"))`

## Build, Test, and Development Workflow

### Building the Project

```bash
# Restore dependencies
dotnet restore BaseWebApplication/BaseWebApplication.sln

# Build the solution
dotnet build BaseWebApplication/BaseWebApplication.sln

# Build without restore (in CI)
dotnet build --no-restore BaseWebApplication/BaseWebApplication.sln
```

### Running Tests

```bash
# Run specific project tests
dotnet test Comment/Tests/Comment.Tests/Comment.Tests.csproj
dotnet test Template/Tests/Template.Tests/Template.Tests.csproj

# Run with specific options (CI)
dotnet test --no-build --verbosity normal Comment/Tests/Comment.Tests/Comment.Tests.csproj
```

### Running Services

```bash
# Run Comment microservice
cd Comment/WebApiComment
dotnet run

# Run Template microservice
cd Template/WebApiTemplate
dotnet run
```

### Postman API Testing

Each microservice includes Postman test collections:

- **Location**: In the respective `WebApi*` folder
- **Files**:
  - `*.postman_collection.json` - Test collection
  - `*.postman_environment.json` - Environment configuration
  - `POSTMAN_TESTS_README.md` - Documentation

**Running Postman tests with Newman:**

```bash
# Comment microservice
newman run Comment/WebApiComment/WebApiComment.postman_collection.json \
  -e Comment/WebApiComment/WebApiComment.postman_environment.json

# Template microservice  
newman run Template/WebApiTemplate/WebApiTemplate.postman_collection.json \
  -e Template/WebApiTemplate/WebApiTemplate.postman_environment.json
```

## CI/CD Pipeline

### GitHub Actions Workflows

1. **Test.yml** - Runs on pull requests to main
   - Builds the solution
   - Runs all tests (Template and Comment)

2. **Comment.yml** - Runs on push when Comment folder changes
   - Builds and tests Comment microservice

3. **TemplateBuild.yml** - Runs on push when Template folder changes
   - Builds and tests Template microservice

### Workflow Steps

All workflows follow the same pattern:
1. Checkout code
2. Setup .NET 8.0.x
3. Restore dependencies from BaseWebApplication.sln
4. Build with `--no-restore`
5. Test with `--no-build --verbosity normal`

## Database Configuration

- **Development**: Uses SQL Server with automatic migrations and data seeding
- **Testing**: Skips SQL Server configuration (uses in-memory or test doubles)
- **Connection String**: Retrieved from `appsettings.json` via `GetConnectionString("DefaultConnection")`

### Database Seeding

- Only seeds data in Development environment
- Uses `UseSeeding` for synchronous seeding
- Uses `UseAsyncSeeding` for running migrations
- Always checks if data exists before seeding to avoid duplicates

## Adding New Features

### Creating a New Microservice

Follow the existing structure:
1. Create `{Name}.Domain` project with models, DTOs, interfaces, and mappers
2. Create `{Name}.Service` project with service implementations
3. Create `{Name}.DataAccess.MsSql` project with repositories and DbContext
4. Create `WebApi{Name}` project with controllers and Program.cs
5. Create `Tests/{Name}.Tests` project with unit/integration tests
6. Add to BaseWebApplication.sln
7. Create GitHub Actions workflow in `.github/workflows/{Name}.yml`
8. Add Postman collection for API testing

### Adding New Endpoints

1. Define service interface in Domain layer
2. Implement service in Service layer
3. Create controller in API layer
4. Add integration tests
5. Update Postman collection

### Adding New Entities

1. Create entity class in `{Service}.Domain/Models/`
2. Create corresponding DTO in `{Service}.Domain/DTOs/`
3. Create mapper in `{Service}.Domain/Mappers/`
4. Define repository interface in `{Service}.Domain/Repositories/`
5. Implement repository in `{Service}.DataAccess.MsSql/Repositories/`
6. Add DbSet to DbContext
7. Create and apply EF Core migration

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
- Template service: Use different prefixes (e.g., `20000000-xxxx-xxxx-xxxx-xxxxxxxxxxxx`)

## Project Goals

This is a learning project focused on:
- Microservices architecture
- Clean architecture principles
- Entity Framework Core
- Repository and Unit of Work patterns
- Integration testing
- CI/CD with GitHub Actions
