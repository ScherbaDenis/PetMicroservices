# WebApp and API Gateway Integration

This document explains how the WebApp is configured to use the API Gateway directly without YARP.

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
│  │ Uses         │         │ Calls Gateway    │    │
│  │ ApiEndpoints │         │ Directly         │    │
│  └──────┬───────┘         └─────────┬────────┘    │
│         │                           │              │
└─────────┼───────────────────────────┼──────────────┘
          │                           │
          │  HTTP Requests            │  HTTP Requests
          │  to Gateway               │  to Gateway
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
- C# backend: Calls Gateway directly (http://localhost:5000)
- TypeScript: Calls Gateway directly using centralized config
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

**Centralized Configuration (apiConfig.ts):**
```typescript
/**
 * API Configuration
 * Centralized configuration for API endpoints
 */
export const API_GATEWAY_URL = 'http://localhost:5000';

/**
 * Helper function to build full API URLs
 */
export function buildApiUrl(path: string): string {
    const normalizedPath = path.startsWith('/') ? path : `/${path}`;
    return `${API_GATEWAY_URL}${normalizedPath}`;
}
```

**TypeScript Usage (templateForm.ts):**
```typescript
import { buildApiUrl } from './apiConfig';

async fetchUsers(): Promise<UserDto[]> {
    const response = await fetch(buildApiUrl('/user'), {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        },
    });

    return await response.json();
}
```

**Request Flow:**
1. TypeScript calls: `buildApiUrl('/user')` → `'http://localhost:5000/user'`
2. Fetch request goes directly to API Gateway
3. API Gateway routes to Template Service
4. Response flows back to TypeScript

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
- **TypeScript Frontend** → API Gateway (direct HTTP calls via centralized config)

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

3. **TypeScript Files**
   - Created: `apiConfig.ts` with centralized API Gateway URL
   - Updated: All TypeScript files to import and use `buildApiUrl()` helper
   - Files: `templateForm.ts`, `answerForm.ts`, `userList.ts`, `templateDetails.ts`, `userDetails.ts`

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

**Test TypeScript Frontend (via API Gateway):**
```bash
# TypeScript calls Gateway directly
curl http://localhost:5000/user
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
**Solution:** 
- Verify API Gateway is running on port 5000
- Check `apiConfig.ts` has correct `API_GATEWAY_URL`
- Ensure backend services are running

### Issue: CORS errors
**Solution:** Should not happen! TypeScript now calls Gateway directly, not cross-origin.

## Future Enhancements

Possible improvements with this architecture:

1. **Add Authentication** - JWT validation at gateway level
2. **Add Rate Limiting** - Throttle requests at gateway
3. **Add Caching** - Cache responses at gateway
4. **Add Logging** - Centralized request/response logging
5. **Add Circuit Breaker** - Protect against service failures
6. **Add Load Balancing** - Distribute load across service instances
