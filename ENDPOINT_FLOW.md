# User Templates Endpoint Flow

## Architecture

The WebApp acts as a **simple proxy** with no validation or business logic. JavaScript calls the WebApp endpoint, which forwards the request directly to the Template microservice.

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
│         │ *** SIMPLE PROXY - NO VALIDATION/LOGIC ***           │
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

## Key Point: WebApp is a Simple Proxy

The WebApp endpoint **does NOT**:
- ❌ Validate if the user exists
- ❌ Check permissions
- ❌ Transform or process the data
- ❌ Add extra fields to the response

The WebApp endpoint **only**:
- ✅ Forwards the request to the Template microservice
- ✅ Returns the response as-is

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

### 4. WebApp controller acts as simple proxy
```csharp
// Simple proxy - no validation, no logic, just forward the request
[HttpGet]
public async Task<IActionResult> GetTemplates(Guid id, CancellationToken cancellationToken)
{
    var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
    return Json(templates);
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

### 7. Response flows back (unchanged from microservice)
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
    "topic": { "name": "Technology" },
    "tags": [{ "name": "Survey" }]
  }
]
```

**Note:** WebApp returns the exact response from the microservice without modification.

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

1. **Simple Proxy Pattern**: WebApp acts as a passthrough with no business logic
2. **CORS Handling**: Properly configured at both layers
3. **Type Safety**: TypeScript on client, C# on server
4. **Separation of Concerns**: WebApp handles routing, Template microservice handles business logic
5. **Flexibility**: Easy to add caching or monitoring at WebApp layer if needed later
6. **No Validation Overhead**: Microservice handles all validation and business rules

## Testing

1. Start Template API: `cd Template/WebApiTemplate && dotnet run`
2. Start WebApp: `cd BaseWebApplication/WebApp && dotnet run`
3. Navigate to: `https://localhost:5001/User/Details/11111111-1111-1111-1111-111111111111`
4. Templates should load automatically in the table
5. Test download buttons (JSON/CSV)
