# User Templates Endpoint Flow

## Architecture

JavaScript calls the Template microservice **directly** via the YARP reverse proxy. There is no custom WebApp endpoint - the WebApp only provides the YARP proxy configuration.

```
┌─────────────────────────────────────────────────────────────────┐
│                      Browser (Client)                           │
│                                                                 │
│  User Details Page (/User/Details/{userId})                    │
│         │                                                       │
│         │ JavaScript (userDetails.js)                          │
│         │                                                       │
│         ▼                                                       │
│  fetch('/proxy/template/user/{userId}')                        │
└─────────────────────────┬───────────────────────────────────────┘
                          │
                          │ HTTP GET (CORS enabled)
                          │
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                    WebApp (ASP.NET Core)                        │
│                                                                 │
│              YARP Reverse Proxy Configuration                   │
│         (No custom controller logic - just routing)             │
│                                                                 │
│  Route: /proxy/template/** → https://localhost:7263/api/template/**
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

## Key Point: Direct Proxy via YARP

The WebApp **does NOT have** a custom UserController endpoint for templates.

Instead:
- ✅ JavaScript calls `/proxy/template/user/{userId}` directly
- ✅ YARP reverse proxy forwards the request to Template microservice
- ✅ No custom controller logic in WebApp
- ✅ All business logic handled by the microservice

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

### 3. YARP proxy forwards to microservice
```
Request: GET /proxy/template/user/11111111-1111-1111-1111-111111111111
Proxied to: GET https://localhost:7263/api/template/user/11111111-1111-1111-1111-111111111111
```

### 4. WebApiTemplate returns data
```
GET https://localhost:7263/api/template/user/11111111-1111-1111-1111-111111111111
```

### 5. Response flows back (unchanged from microservice)
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

### 6. JavaScript displays templates
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
| `TypeScript/userDetails.ts` | Client-side logic calling `/proxy/template/user/{id}` |
| `Views/User/Details.cshtml` | UI displaying templates table |
| `Program.cs` | YARP reverse proxy configuration |
| `Template/WebApiTemplate/Controllers/TemplateController.cs` | API endpoint in microservice |

**Note:** There is NO UserController.GetTemplates endpoint. The YARP proxy handles all routing.

## Benefits of This Architecture

1. **Pure YARP Proxy**: WebApp only provides routing configuration, no custom logic
2. **Direct Communication**: JavaScript talks directly to microservice (via proxy)
3. **CORS Handling**: Configured at microservice and YARP level
4. **Simplified WebApp**: No unnecessary controller endpoints
5. **Separation of Concerns**: Microservice handles all business logic
6. **Reduced Latency**: One less hop (no controller processing)

## Testing

1. Start Template API: `cd Template/WebApiTemplate && dotnet run`
2. Start WebApp: `cd BaseWebApplication/WebApp && dotnet run`
3. Navigate to: `https://localhost:5001/User/Details/11111111-1111-1111-1111-111111111111`
4. Templates should load automatically in the table
5. Test download buttons (JSON/CSV)
