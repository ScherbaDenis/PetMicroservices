# WebApp and API Gateway Integration

This document explains how the WebApp is configured to use the API Gateway and why CORS was removed.

## Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                    WebApp                           │
│                 (Port 5177/7200)                    │
│                                                     │
│  ┌──────────────┐         ┌──────────────────┐    │
│  │  C# Backend  │         │  TypeScript      │    │
│  │  Services    │         │  Frontend        │    │
│  │              │         │                  │    │
│  │ Uses         │         │ Uses YARP        │    │
│  │ ApiEndpoints │         │ /proxy/ routes   │    │
│  └──────┬───────┘         └─────────┬────────┘    │
│         │                           │              │
└─────────┼───────────────────────────┼──────────────┘
          │                           │
          │  HTTP Requests            │  Same-origin
          │  to Gateway               │  requests
          ▼                           ▼
    ┌─────────────────────────────────────┐
    │        API Gateway                  │
    │        (Port 5000)                  │
    │                                     │
    │  Ocelot Routes:                     │
    │  - /user → Template Service         │
    │  - /topic → Template Service        │
    │  - /template → Template Service     │
    │  - /tag → Template Service          │
    │  - /comment → Comment Service       │
    │  - /question → Answer Service       │
    │  - /answer → Answer Service         │
    └─────────────┬───────────────────────┘
                  │
         ┌────────┼────────┬──────────┐
         ▼        ▼        ▼          ▼
    ┌─────────┐ ┌────────┐ ┌──────────┐
    │Template │ │Comment │ │  Answer  │
    │Service  │ │Service │ │  Service │
    │Port 5100│ │Port 5234│ │Port 5136│
    └─────────┘ └────────┘ └──────────┘
```

## Why CORS Was Removed

### Before (With CORS)
- WebApp called services directly
- Services ran on different ports (cross-origin)
- CORS was required to allow cross-origin requests
- Each service needed CORS configuration

### After (Without CORS)
- WebApp uses API Gateway as single entry point
- All requests flow through the gateway
- C# backend: Same-origin (WebApp → Gateway)
- TypeScript: Uses YARP proxy (same-origin within WebApp)
- **No cross-origin requests = No CORS needed**

## Request Flow Details

### 1. C# Backend Services

**Configuration (appsettings.json):**
```json
{
  "ApiEndpoints": {
    "UserService": "http://localhost:5000/user",
    "TopicService": "http://localhost:5000/topic",
    "TemplateService": "http://localhost:5000/template",
    "QuestionService": "http://localhost:5000/question",
    "TagService": "http://localhost:5000/tag",
    "CommentService": "http://localhost:5000/comment",
    "AnswerService": "http://localhost:5000"
  }
}
```

**Example Service (TemplateService.cs):**
```csharp
public class TemplateService : ITemplateService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public TemplateService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        // Reads: http://localhost:5000/template
        _baseUrl = configuration["ApiEndpoints:TemplateService"];
    }

    public async Task<TemplateDto> GetByIdAsync(Guid id)
    {
        // Calls: GET http://localhost:5000/template/{id}
        var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
        return await response.Content.ReadFromJsonAsync<TemplateDto>();
    }
}
```

### 2. TypeScript Frontend

**YARP Reverse Proxy Configuration:**
```json
{
  "ReverseProxy": {
    "Routes": {
      "template-route": {
        "ClusterId": "template-cluster",
        "Match": {
          "Path": "/proxy/template/{**catch-all}"
        },
        "Transforms": [
          {
            "PathPattern": "/api/template/{**catch-all}"
          }
        ]
      }
    },
    "Clusters": {
      "template-cluster": {
        "Destinations": {
          "template-service": {
            "Address": "https://localhost:7263"
          }
        }
      }
    }
  }
}
```

**TypeScript Usage (templateForm.ts):**
```typescript
async fetchUsers(): Promise<UserDto[]> {
    const response = await fetch('/proxy/user', {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
        // NO mode: 'cors' needed - same-origin request
    });

    return await response.json();
}
```

**Request Flow:**
1. TypeScript calls: `fetch('/proxy/user')`
2. YARP intercepts: `/proxy/user` → routes to Template Service
3. YARP calls: `https://localhost:7263/api/user`
4. Response flows back through YARP to TypeScript

## Benefits of This Architecture

### 1. **No CORS Configuration Required**
- ✅ Simpler configuration
- ✅ No CORS security concerns
- ✅ No pre-flight OPTIONS requests

### 2. **Centralized Routing**
- ✅ Single point of entry (API Gateway)
- ✅ Easier to manage service endpoints
- ✅ Can add authentication/rate limiting at gateway

### 3. **Two Integration Paths**
- **C# Backend** → API Gateway (direct HTTP calls)
- **TypeScript Frontend** → YARP → Services (proxied through WebApp)

### 4. **Flexibility**
- Can switch service endpoints by changing gateway config
- Can add load balancing
- Can add circuit breakers
- Can add request/response transformation

## Configuration Summary

### Files Modified

1. **Program.cs**
   - Removed: `builder.Services.AddCors()`
   - Removed: `app.UseCors("DefaultCorsPolicy")`

2. **appsettings.json**
   - Updated: All `ApiEndpoints` point to API Gateway (port 5000)
   - Removed: `"CorsPolicy"` from all YARP routes

3. **TypeScript Files**
   - Removed: `mode: 'cors'` from all fetch calls
   - Files: `templateForm.ts`, `answerForm.ts`, `userList.ts`

## Development Setup

### Starting the Services

1. **Start API Gateway:**
   ```bash
   cd ApiGateway/ApiGateway
   dotnet run
   # Runs on http://localhost:5000
   ```

2. **Start Template Service:**
   ```bash
   cd Template/WebApiTemplate
   dotnet run
   # Runs on https://localhost:7263 (or http://localhost:5100)
   ```

3. **Start Comment Service:**
   ```bash
   cd Comment/WebApiComment
   dotnet run
   # Runs on https://localhost:7115 (or http://localhost:5234)
   ```

4. **Start Answer Service:**
   ```bash
   cd Answer/src/Answer.Api
   dotnet run
   # Runs on https://localhost:7065 (or http://localhost:5136)
   ```

5. **Start WebApp:**
   ```bash
   cd BaseWebApplication/WebApp
   dotnet run
   # Runs on https://localhost:7200 or http://localhost:5177
   ```

### Testing

**Test C# Backend (via API Gateway):**
```bash
# Through WebApp's TemplateService
curl http://localhost:5177/Template
```

**Test TypeScript Frontend (via YARP):**
```bash
# Through WebApp's YARP proxy
curl http://localhost:5177/proxy/user
```

**Test API Gateway Directly:**
```bash
curl http://localhost:5000/user
curl http://localhost:5000/template
curl http://localhost:5000/comment
```

## Troubleshooting

### Issue: 404 Not Found from C# services
**Solution:** Verify API Gateway is running on port 5000

### Issue: TypeScript fetch fails
**Solution:** Check YARP routes in appsettings.json, ensure backend services are running

### Issue: CORS errors
**Solution:** Should not happen anymore! If you see CORS errors, verify:
- No `mode: 'cors'` in TypeScript fetch calls
- No CORS middleware in Program.cs
- Using correct endpoints (through gateway or YARP)

## Future Enhancements

Possible improvements with this architecture:

1. **Add Authentication** - JWT validation at gateway level
2. **Add Rate Limiting** - Throttle requests at gateway
3. **Add Caching** - Cache responses at gateway
4. **Add Logging** - Centralized request/response logging
5. **Add Circuit Breaker** - Protect against service failures
6. **Add Load Balancing** - Distribute load across service instances
