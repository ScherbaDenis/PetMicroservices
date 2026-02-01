# GetByUserIdAsync Method Usage Guide

## Overview

The `GetByUserIdAsync` method in `ITemplateService` retrieves all templates associated with a specific user. This is useful for displaying a user's templates, filtering data, or generating user-specific reports.

## Method Signature

```csharp
Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
```

## Implementation Details

### Location
- **Interface**: `BaseWebApplication/WebApp/Services/ITemplateService.cs`
- **Implementation**: `BaseWebApplication/WebApp/Services/Imp/TemplateService.cs`

### How It Works

The method calls the Template microservice API endpoint:
```
GET /api/template/user/{userId}
```

This endpoint is proxied through YARP at:
```
GET /proxy/template/user/{userId}
```

## Current Usage

### 1. Client-Side (TypeScript) - User Details Page

The primary usage is in the User Details view where templates are fetched and displayed dynamically.

**File**: `TypeScript/userDetails.ts`

```typescript
class UserTemplateManager {
    async fetchUserTemplates(): Promise<TemplateDto[]> {
        const response = await fetch(`${this.apiBaseUrl}/user/${this.userId}`, {
            method: 'GET',
            headers: { 'Content-Type': 'application/json' },
            mode: 'cors',
        });
        return await response.json();
    }
}
```

**View**: `Views/User/Details.cshtml`

The User Details page displays:
- User information
- Templates table (Title, Description, Topic, Tags)
- Download buttons (JSON, CSV)
- Refresh functionality

### 2. Server-Side Example

You can use this method in your controllers to fetch user templates server-side:

```csharp
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ITemplateService _templateService;

    // GET: /User/Details/5
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user == null) return NotFound();
        
        // Fetch templates for this user
        var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
        
        // Pass to view or return as JSON
        ViewBag.Templates = templates;
        return View(user);
    }
}
```

### 3. API Usage Example

Direct API call using HttpClient:

```csharp
var httpClient = new HttpClient();
var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
var response = await httpClient.GetAsync($"https://localhost:7263/api/template/user/{userId}");
var templates = await response.Content.ReadFromJsonAsync<IEnumerable<TemplateDto>>();
```

## Use Cases

1. **User Dashboard**: Display all templates created by or assigned to a user
2. **Template Management**: Filter templates by ownership
3. **Reports**: Generate user-specific template reports
4. **Export**: Download user's templates as JSON/CSV
5. **Access Control**: Verify user has access to specific templates

## Response Format

```json
[
  {
    "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "title": "Customer Feedback Survey",
    "description": "A template for collecting customer feedback",
    "owner": {
      "id": "11111111-1111-1111-1111-111111111111",
      "name": "John Doe"
    },
    "topic": {
      "id": "...",
      "name": "Technology"
    },
    "tags": [
      {
        "id": "...",
        "name": "Survey"
      }
    ]
  }
]
```

## Error Handling

```csharp
try
{
    var templates = await _templateService.GetByUserIdAsync(userId, cancellationToken);
    // Process templates
}
catch (HttpRequestException ex)
{
    // Handle API communication errors
    _logger.LogError(ex, "Failed to fetch templates for user {UserId}", userId);
    return StatusCode(500, "Failed to retrieve templates");
}
```

## Testing

Tests are available in:
- `Template/Tests/Template.Tests/Controllers/TemplateControllerTests.cs`

Example test:
```csharp
[Fact]
public async Task GetByUserId_ShouldReturnOkWithTemplates_WhenUserExists()
{
    // Arrange
    var userId = Guid.NewGuid();
    var templates = new List<TemplateDto> { ... };
    _mockService.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
               .ReturnsAsync(templates);
    
    // Act
    var result = await _controller.GetByUserId(userId);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    Assert.Equal(2, ((IEnumerable<TemplateDto>)okResult.Value).Count());
}
```

## Related Files

- `/BaseWebApplication/WebApp/Services/ITemplateService.cs` - Interface definition
- `/BaseWebApplication/WebApp/Services/Imp/TemplateService.cs` - Implementation
- `/BaseWebApplication/WebApp/TypeScript/userDetails.ts` - Client-side usage
- `/BaseWebApplication/WebApp/Views/User/Details.cshtml` - UI implementation
- `/Template/WebApiTemplate/Controllers/TemplateController.cs` - API endpoint
- `/Template/Tests/Template.Tests/Controllers/TemplateControllerTests.cs` - Tests

## CORS Configuration

The Template API has CORS enabled to allow cross-origin requests from the WebApp:

```csharp
// In Template/WebApiTemplate/Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        else
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
    });
});
```

## See Also

- [User Details View Implementation](Views/User/Details.cshtml)
- [TypeScript Template Manager](TypeScript/userDetails.ts)
- [Template API Documentation](../../Template/WebApiTemplate/Controllers/TemplateController.cs)
