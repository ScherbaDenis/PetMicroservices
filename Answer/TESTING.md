# Testing Guide

This document describes the comprehensive testing strategy for the Answer API project.

## Test Structure

The project follows a layered testing approach:

```
tests/
├── Answer.Domain.Tests/           # Unit tests for domain entities
├── Answer.Infrastructure.Tests/   # Unit tests for repositories
└── Answer.Api.IntegrationTests/   # Integration tests for API endpoints

postman/
├── Answer_API_Tests.postman_collection.json     # Postman test collection
├── Answer_API_Development.postman_environment.json  # Environment config
└── README.md                                    # Postman usage guide
```

## Running Tests

### All Tests

Run all tests in the solution:
```bash
dotnet test
```

Run with detailed output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Specific Test Projects

**Domain Tests:**
```bash
dotnet test tests/Answer.Domain.Tests/Answer.Domain.Tests.csproj
```

**Infrastructure Tests:**
```bash
dotnet test tests/Answer.Infrastructure.Tests/Answer.Infrastructure.Tests.csproj
```

**Integration Tests:**
```bash
dotnet test tests/Answer.Api.IntegrationTests/Answer.Api.IntegrationTests.csproj
```

### Test Coverage

Generate test coverage report:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Unit Tests

### Domain Tests (15 tests)

Located in `tests/Answer.Domain.Tests/Entities/`

**User Tests (4 tests):**
- ID generation and uniqueness
- Name initialization and modification
- Property validation

**Question Tests (3 tests):**
- ID generation
- Title initialization and modification
- Property validation

**Answer Tests (8 tests):**
- ID generation
- Property initialization
- All AnswerType enumerations
- Relationship properties

**Run Domain Tests:**
```bash
cd tests/Answer.Domain.Tests
dotnet test
```

### Infrastructure Tests (15 tests)

Located in `tests/Answer.Infrastructure.Tests/Repositories/`

**InMemoryRepository Tests (8 tests):**
- Add entity
- Get by ID (exists and not exists)
- Get all entities
- Update entity
- Delete entity (exists and not exists)
- Thread-safety validation

**MsSqlRepository Tests (7 tests):**
- Add entity with database persistence
- Get by ID (exists and not exists)
- Get all entities
- Update entity with database sync
- Delete entity (exists and not exists)

**Run Infrastructure Tests:**
```bash
cd tests/Answer.Infrastructure.Tests
dotnet test
```

## Integration Tests (6+ tests)

Located in `tests/Answer.Api.IntegrationTests/Controllers/`

**User Endpoints Tests (6 tests):**
- Create user via REST API
- Get all users
- Get user by ID
- Update user
- Delete user
- Invalid ID handling

**Features:**
- Uses `WebApplicationFactory` for in-memory testing
- Tests actual HTTP endpoints
- Validates JSON responses
- Tests complete request/response cycle

**Run Integration Tests:**
```bash
cd tests/Answer.Api.IntegrationTests
dotnet test
```

## Postman Tests

Comprehensive API testing via Postman with automated test scripts.

**Coverage:**
- 21+ endpoint tests
- 5 entity types (Users, Questions, Templates, Answers, Health)
- Automated ID capturing and reuse
- Response validation
- Status code verification

**Run with Newman (CLI):**
```bash
# Install Newman
npm install -g newman

# Run tests
cd postman
newman run Answer_API_Tests.postman_collection.json \
  -e Answer_API_Development.postman_environment.json
```

**Generate HTML Report:**
```bash
npm install -g newman-reporter-htmlextra

newman run Answer_API_Tests.postman_collection.json \
  -e Answer_API_Development.postman_environment.json \
  -r htmlextra \
  --reporter-htmlextra-export postman-report.html
```

See `postman/README.md` for detailed Postman testing instructions.

## Test Frameworks and Libraries

### .NET Testing Stack

- **xUnit** 2.8.2+ - Test framework
- **FluentAssertions** 8.8.0+ - Fluent assertion library
- **Moq** - Mocking framework (for future use)
- **Microsoft.AspNetCore.Mvc.Testing** 9.0.0 - Integration testing
- **Microsoft.EntityFrameworkCore.InMemory** 9.0.0 - EF Core in-memory database

### Postman/Newman

- **Newman** - Postman CLI runner
- **newman-reporter-htmlextra** - HTML report generator

## Writing New Tests

### Unit Test Template

```csharp
using FluentAssertions;
using Xunit;

namespace Answer.Domain.Tests.Entities;

public class NewEntityTests
{
    [Fact]
    public void NewEntity_ShouldHaveExpectedBehavior()
    {
        // Arrange
        var entity = new NewEntity();

        // Act
        var result = entity.SomeMethod();

        // Assert
        result.Should().NotBeNull();
        result.SomeProperty.Should().Be("expected value");
    }

    [Theory]
    [InlineData("value1", "expected1")]
    [InlineData("value2", "expected2")]
    public void NewEntity_ShouldHandle_ParameterizedTests(string input, string expected)
    {
        // Arrange
        var entity = new NewEntity();

        // Act
        var result = entity.ProcessInput(input);

        // Assert
        result.Should().Be(expected);
    }
}
```

### Integration Test Template

```csharp
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Answer.Api.IntegrationTests.Controllers;

public class NewEndpointTests : IClassFixture<AnswerApiFactory>
{
    private readonly HttpClient _client;

    public NewEndpointTests(AnswerApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task NewEndpoint_ShouldReturnExpectedResult()
    {
        // Arrange
        var request = new { property = "value" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/newentity", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("expected");
    }
}
```

## Continuous Integration

### GitHub Actions Example

```yaml
name: Tests

on: [push, pull_request]

jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '9.0.x'
      
      - name: Run Unit Tests
        run: dotnet test --filter "FullyQualifiedName~Domain|FullyQualifiedName~Infrastructure"
  
  integration-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '9.0.x'
      
      - name: Run Integration Tests
        run: dotnet test tests/Answer.Api.IntegrationTests/
  
  api-tests:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '9.0.x'
      
      - name: Start API
        run: |
          cd src/Answer.Api
          dotnet run &
          sleep 15
      
      - name: Run Postman Tests
        uses: matt-ball/newman-action@v2
        with:
          collection: postman/Answer_API_Tests.postman_collection.json
          environment: postman/Answer_API_Development.postman_environment.json
```

## Test Best Practices

### 1. Test Naming

Use descriptive names that explain what is being tested:
```csharp
[Fact]
public void EntityMethod_ShouldReturnExpectedValue_WhenConditionIsMet()
```

### 2. AAA Pattern

Structure tests with Arrange-Act-Assert:
```csharp
// Arrange - Set up test data and conditions
var entity = new Entity();

// Act - Execute the method being tested
var result = entity.DoSomething();

// Assert - Verify the outcome
result.Should().Be(expected);
```

### 3. One Assertion Per Test

Focus each test on a single behavior:
```csharp
// Good
[Fact]
public void User_ShouldHaveGuidId_WhenCreated()
{
    var user = new User();
    user.Id.Should().NotBeEmpty();
}

// Avoid multiple unrelated assertions in one test
```

### 4. Use Theory for Parameterized Tests

```csharp
[Theory]
[InlineData(AnswerType.SingleLineString)]
[InlineData(AnswerType.MultiLineText)]
[InlineData(AnswerType.PositiveInteger)]
public void Answer_ShouldAcceptAllAnswerTypes(AnswerType type)
{
    var answer = new Answer { AnswerType = type };
    answer.AnswerType.Should().Be(type);
}
```

### 5. Test Both Success and Failure Cases

```csharp
[Fact]
public void GetById_ShouldReturnEntity_WhenExists() { }

[Fact]
public void GetById_ShouldReturnNull_WhenNotExists() { }
```

## Test Maintenance

### Keeping Tests Green

1. Run tests before committing code
2. Fix failing tests immediately
3. Update tests when requirements change
4. Remove obsolete tests

### Test Performance

- Keep unit tests fast (< 100ms each)
- Use in-memory databases for integration tests
- Mock external dependencies
- Run expensive tests separately

### Test Coverage Goals

- **Unit Tests**: Aim for 80%+ code coverage
- **Integration Tests**: Cover all API endpoints
- **Postman Tests**: Validate end-to-end workflows

## Troubleshooting

### Tests Fail Locally

1. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet build
   dotnet test
   ```

2. Check for port conflicts (API integration tests use port 5136)

3. Ensure all dependencies are restored:
   ```bash
   dotnet restore
   ```

### Tests Pass Locally But Fail in CI

1. Check for hardcoded paths or URLs
2. Verify environment variables
3. Ensure database migrations are applied
4. Check for timing issues (add appropriate delays)

### Flaky Tests

1. Identify non-deterministic behavior
2. Add proper synchronization
3. Use async/await correctly
4. Avoid shared state between tests

## Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Postman Learning Center](https://learning.postman.com/)
- [Newman Documentation](https://learning.postman.com/docs/running-collections/using-newman-cli/command-line-integration-with-newman/)
