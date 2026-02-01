# User Templates Endpoint Flow

## Architecture

The application uses a clean separation where JavaScript calls a WebApp endpoint, which then calls the Template microservice API with proper CORS handling.

```
┌─────────────────────────────────────────────────────────────────┐
│                      Browser (Client)                           │
│                                                                 │
│  User Details Page (/User/Details/{userId})                    │
│         │                                                       │
│         │ JavaScript (userDetails.js)                          │
│         │                                                       │
│         ▼                                                       │
│  fetch('/User/GetTemplates/{userId}')                          │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          │ HTTP GET
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                    WebApp (ASP.NET Core)                        │
│                                                                 │
│  UserController.GetTemplates(userId)                            │
│         │                                                       │
│         │ CORS enabled (DefaultCorsPolicy)                     │
│         │                                                       │
│         ▼                                                       │
│  _templateService.GetByUserIdAsync(userId)                     │
│         │                                                       │
│         │ TemplateService (HttpClient)                         │
│         │                                                       │
│         ▼                                                       │
│  HTTP GET to /api/template/user/{userId}                       │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          │ HTTP GET
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│              WebApiTemplate (Template Microservice)             │
│                                                                 │
│  TemplateController.GetByUserId(userId)                        │
│         │                                                       │
│         │ CORS enabled (environment-based)                     │
│         │                                                       │
│         ▼                                                       │
│  Returns: IEnumerable<TemplateDto>                             │
└─────────────────────────────────────────────────────────────────┘
```

## Request Flow Example

### 1. User navigates to Details page
```
GET /User/Details/11111111-1111-1111-1111-111111111111
```

### 2. JavaScript executes on page load
```javascript
const templateManager = new UserTemplateManager(userId);
templateManager.displayTemplates();
```

### 3. JavaScript calls WebApp endpoint
```
GET /User/GetTemplates/11111111-1111-1111-1111-111111111111
```

### 4. WebApp controller processes request
```csharp
[HttpGet]
public async Task<IActionResult> GetTemplates(Guid id, CancellationToken cancellationToken)
{
    var user = await _service.GetByIdAsync(id, cancellationToken);
    if (user == null) return NotFound(new { error = "User not found" });

    var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
    
    return Json(new 
    { 
        userId = user.Id, 
        userName = user.Name, 
        templates = templates,
        count = templates.Count()
    });
}
```

### 5. TemplateService calls microservice
```csharp
public async Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
{
    var response = await _httpClient.GetAsync($"{_baseUrl}/user/{userId}", cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<IEnumerable<TemplateDto>>(_jsonOptions, cancellationToken)
           ?? Enumerable.Empty<TemplateDto>();
}
```

### 6. WebApiTemplate returns data
```
GET https://localhost:7263/api/template/user/11111111-1111-1111-1111-111111111111
```

### 7. Response flows back
```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "userName": "John Doe",
  "templates": [
    {
      "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "title": "Customer Feedback Survey",
      "description": "A template for collecting customer feedback"
    }
  ],
  "count": 1
}
```

### 8. JavaScript displays templates
The templates are rendered in the table on the User Details page.

## CORS Configuration

### WebApp (BaseWebApplication/WebApp/Program.cs)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ...

app.UseCors("DefaultCorsPolicy");
```

### WebApiTemplate (Template/WebApiTemplate/Program.cs)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                ?? new[] { "https://localhost:5001" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// ...

app.UseCors("DefaultCorsPolicy");
```

## Key Files

| File | Purpose |
|------|---------|
| `TypeScript/userDetails.ts` | Client-side logic to fetch templates |
| `Controllers/UserController.cs` | WebApp endpoint for getting user templates |
| `Services/ITemplateService.cs` | Service interface |
| `Services/Imp/TemplateService.cs` | Service implementation calling Template API |
| `Views/User/Details.cshtml` | UI displaying templates table |
| `Template/WebApiTemplate/Controllers/TemplateController.cs` | API endpoint in microservice |

## Benefits of This Architecture

1. **Separation of Concerns**: WebApp handles presentation, Template microservice handles data
2. **CORS Handling**: Properly configured at both layers
3. **Type Safety**: TypeScript on client, C# on server
4. **Testability**: Each layer can be tested independently
5. **Security**: WebApp can add authentication/authorization before calling microservice
6. **Flexibility**: Easy to add caching, logging, or validation at WebApp layer

## Testing

1. Start Template API: `cd Template/WebApiTemplate && dotnet run`
2. Start WebApp: `cd BaseWebApplication/WebApp && dotnet run`
3. Navigate to: `https://localhost:5001/User/Details/11111111-1111-1111-1111-111111111111`
4. Templates should load automatically in the table
5. Test download buttons (JSON/CSV)
