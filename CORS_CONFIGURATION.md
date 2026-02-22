# CORS Configuration

This document describes the Cross-Origin Resource Sharing (CORS) configuration across all services in the PetMicroservices architecture.

## Overview

CORS is configured on all services to allow cross-origin requests from the WebApp and between services. The configuration uses environment-based policies for flexibility between development and production environments.

## Services Configuration

### API Gateway (Port 5000)

**Policy Name:** `ApiGatewayCorsPolicy`

**Development:**
- Allows requests from any origin
- Allows all HTTP methods
- Allows all headers

**Production:**
- Restricts to specific origins from configuration
- Default allowed origins: `https://localhost:7200`, `https://localhost:5177`
- Configure via `AllowedOrigins` in appsettings.json

**Location:** `ApiGateway/ApiGateway/Program.cs`

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiGatewayCorsPolicy", policy =>
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
                ?? new[] { "https://localhost:7200", "https://localhost:5177" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});
```

### Comment Service (Port 5234)

**Policy Name:** `DefaultCorsPolicy`

**Development:**
- Allows requests from any origin
- Allows all HTTP methods
- Allows all headers

**Production:**
- Default allowed origins: `https://localhost:7200`, `http://localhost:5000`
- Configure via `AllowedOrigins` in appsettings.json

**Location:** `Comment/WebApiComment/Program.cs`

### Template Service (Port 5100)

**Policy Name:** `DefaultCorsPolicy`

**Development:**
- Allows requests from any origin
- Allows all HTTP methods
- Allows all headers

**Production:**
- Default allowed origin: `https://localhost:5001`
- Configure via `AllowedOrigins` in appsettings.json

**Location:** `Template/WebApiTemplate/Program.cs`

### Answer Service (Port 5136)

**Policy Name:** `AllowAll`

**Special Configuration for gRPC-Web:**
- Exposes gRPC-specific headers: `Grpc-Status`, `Grpc-Message`, `Grpc-Encoding`, `Grpc-Accept-Encoding`
- Required for browser-based gRPC clients

**Development:**
- Allows requests from any origin
- Allows all HTTP methods
- Allows all headers
- Exposes gRPC headers

**Production:**
- Default allowed origins: `https://localhost:7200`, `http://localhost:5000`
- Configure via `AllowedOrigins` in appsettings.json

**Location:** `Answer/src/Answer.Api/Program.cs`

### WebApp (Port 5177/7200)

**Policy Name:** `DefaultCorsPolicy`

**Configuration:**
- Allows requests from any origin (development)
- Used for serving static assets and proxy routes

**Location:** `BaseWebApplication/WebApp/Program.cs`

## Architecture Diagram

```
┌─────────────────────────────────────────┐
│         WebApp (Port 5177/7200)         │
│         CORS: AllowAnyOrigin             │
└──────────────────┬──────────────────────┘
                   │
                   │ HTTP/HTTPS
                   │
      ┌────────────┴───────────────┐
      │                            │
      ▼                            ▼
┌───────────────┐          ┌──────────────────┐
│  API Gateway  │          │ Direct Service   │
│  Port 5000    │          │    Access        │
│  CORS: ✓      │          │                  │
└───────┬───────┘          └──────────────────┘
        │
        │ Routes to services
        │
   ┌────┼─────┬─────────┬──────────┐
   ▼    ▼     ▼         ▼          ▼
┌─────┐ ┌───────┐ ┌─────────┐ ┌──────────┐
│Comm-│ │Templ- │ │ Answer  │ │  Other   │
│ent  │ │ate    │ │ Service │ │ Services │
│5234 │ │5100   │ │  5136   │ │          │
│CORS:│ │CORS:✓ │ │ CORS:✓  │ │          │
│ ✓   │ │       │ │ gRPC-Web│ │          │
└─────┘ └───────┘ └─────────┘ └──────────┘
```

## Configuration in Production

To configure allowed origins in production, add the following to your `appsettings.json` or environment variables:

```json
{
  "AllowedOrigins": [
    "https://your-webapp-domain.com",
    "https://your-api-gateway-domain.com"
  ]
}
```

## Testing CORS

### Using curl

Test CORS preflight request:

```bash
# Test API Gateway CORS
curl -X OPTIONS http://localhost:5000/comment \
  -H "Origin: http://localhost:5177" \
  -H "Access-Control-Request-Method: GET" \
  -v

# Test Comment Service CORS
curl -X OPTIONS http://localhost:5234/api/comment \
  -H "Origin: http://localhost:5177" \
  -H "Access-Control-Request-Method: GET" \
  -v

# Test Answer Service CORS (gRPC-Web)
curl -X OPTIONS http://localhost:5136/api/answers \
  -H "Origin: http://localhost:5177" \
  -H "Access-Control-Request-Method: POST" \
  -v
```

### From Browser

Open browser console and test:

```javascript
// Test API Gateway
fetch('http://localhost:5000/comment')
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('CORS Error:', error));

// Test direct service access
fetch('http://localhost:5234/api/comment')
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('CORS Error:', error));
```

## Common Issues

### Issue: "Access to fetch at '...' from origin '...' has been blocked by CORS policy"

**Solution:**
1. Verify the service is running
2. Check that CORS policy is configured on the service
3. Ensure the origin is in the allowed origins list for production
4. Check that `app.UseCors()` is called before other middleware in Program.cs

### Issue: gRPC-Web requests fail with CORS errors

**Solution:**
1. Ensure Answer service has gRPC-specific headers exposed
2. Verify `UseGrpcWeb()` is called after `UseCors()`
3. Check that the gRPC service is mapped with `.EnableGrpcWeb()`

### Issue: Preflight OPTIONS requests fail

**Solution:**
1. Ensure CORS is configured before routing middleware
2. Verify that `AllowAnyMethod()` or specific methods include OPTIONS
3. Check that the service responds to OPTIONS requests

## Security Considerations

1. **Development vs Production:** Always use restricted origins in production
2. **HTTPS:** Use HTTPS in production to prevent MITM attacks
3. **Credentials:** If using cookies/auth, add `.AllowCredentials()` to CORS policy
4. **Headers:** Only expose necessary headers to reduce attack surface
5. **Regular Updates:** Keep CORS configuration in sync with deployed origins

## WebApp API Gateway Integration

WebApp can be configured to use either:
1. **Direct service calls** (default) - Each service called directly
2. **API Gateway** - All calls routed through gateway

Configure in `appsettings.json`:

```json
{
  "UseApiGateway": false,
  "ApiGatewayEndpoint": "http://localhost:5000"
}
```

Set `UseApiGateway: true` to route all requests through the API Gateway.
