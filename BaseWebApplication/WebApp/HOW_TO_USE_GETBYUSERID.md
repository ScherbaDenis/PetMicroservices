# GetByUserIdAsync Method - Complete Guide

## Overview

The `GetByUserIdAsync` method was added to `ITemplateService` to retrieve all templates associated with a specific user. This document explains how it's being used throughout the application.

## 📚 Documentation Files

1. **[TEMPLATE_SERVICE_USAGE.md](TEMPLATE_SERVICE_USAGE.md)** - Complete technical documentation
   - Method signature and implementation details
   - Current usage patterns
   - API endpoint information
   - Response format
   - Testing information

2. **[GETBYUSERID_EXAMPLES.md](GETBYUSERID_EXAMPLES.md)** - Practical examples
   - Server-side controller usage
   - Client-side TypeScript usage
   - Direct API calls
   - Export/download examples
   - Error handling patterns

## 🚀 Quick Start

### 1. Server-Side Usage (C#)

The method is now used in `UserController` with a new endpoint:

```csharp
// GET: /User/GetTemplates/{userId}
[HttpGet]
public async Task<IActionResult> GetTemplates(Guid id, CancellationToken cancellationToken)
{
    var user = await _service.GetByIdAsync(id, cancellationToken);
    if (user == null) return NotFound();

    var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
    
    return Json(new { 
        userId = user.Id, 
        userName = user.Name, 
        templates = templates,
        count = templates.Count()
    });
}
```

**Test it**:
```
GET https://localhost:5001/User/GetTemplates/11111111-1111-1111-1111-111111111111
```

### 2. Client-Side Usage (TypeScript)

Used in the User Details page to dynamically load and display templates:

```typescript
// Automatically fetches templates when viewing user details
const templateManager = new UserTemplateManager(userId);
await templateManager.displayTemplates();  // Loads and displays templates
await templateManager.downloadAsJson();    // Downloads as JSON file
await templateManager.downloadAsCsv();     // Downloads as CSV file
```

**See it in action**:
1. Navigate to: `/User/Details/11111111-1111-1111-1111-111111111111`
2. The Templates section loads automatically
3. Use buttons to download or refresh data

## 🎯 Where It's Used

### Current Implementations

| Location | Type | Description |
|----------|------|-------------|
| `Services/ITemplateService.cs` | Interface | Method definition with XML docs |
| `Services/Imp/TemplateService.cs` | Implementation | Calls Template API `/user/{userId}` endpoint |
| `Controllers/UserController.cs` | Server-Side | New `GetTemplates` action for JSON API |
| `TypeScript/userDetails.ts` | Client-Side | Fetches and displays templates in UI |
| `Views/User/Details.cshtml` | View | Displays templates table with download buttons |

### API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/template/user/{userId}` | GET | Template microservice endpoint |
| `/proxy/template/user/{userId}` | GET | YARP proxied endpoint (used by client) |
| `/User/GetTemplates/{userId}` | GET | WebApp controller endpoint (new) |

## 🔍 How It Works

```
┌─────────────────┐
│  User Details   │  Browser navigates to /User/Details/{id}
│      Page       │
└────────┬────────┘
         │
         ├─────────────────────────────────────────┐
         │                                         │
         ▼                                         ▼
┌─────────────────┐                      ┌─────────────────┐
│   Server-Side   │                      │  Client-Side    │
│   (Optional)    │                      │  (TypeScript)   │
└────────┬────────┘                      └────────┬────────┘
         │                                         │
         │ GetByUserIdAsync(userId)                │ fetch('/proxy/template/user/{id}')
         │                                         │
         ▼                                         ▼
┌──────────────────────────────────────────────────────────┐
│            ITemplateService.GetByUserIdAsync             │
│                  (TemplateService.cs)                    │
└────────────────────────┬─────────────────────────────────┘
                         │
                         │ HTTP GET
                         │
                         ▼
┌──────────────────────────────────────────────────────────┐
│         Template Microservice API                        │
│         GET /api/template/user/{userId}                  │
└────────────────────────┬─────────────────────────────────┘
                         │
                         ▼
                   Returns Templates
```

## 📊 Data Flow Example

**Request**:
```
GET /User/Details/11111111-1111-1111-1111-111111111111
```

**What Happens**:
1. Page loads with user information (John Doe)
2. TypeScript executes on page load
3. Fetches templates via: `GET /proxy/template/user/11111111-1111-1111-1111-111111111111`
4. Template service returns user's templates
5. Templates displayed in table with columns: Title, Description, Topic, Tags
6. User can download as JSON or CSV

**Response Example**:
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

## 🧪 Testing

### Manual Testing

1. **Start the applications**:
   ```bash
   # Terminal 1 - Template API
   cd Template/WebApiTemplate
   dotnet run
   
   # Terminal 2 - WebApp
   cd BaseWebApplication/WebApp
   dotnet run
   ```

2. **Test endpoints**:
   - User Details: `https://localhost:5001/User/Details/11111111-1111-1111-1111-111111111111`
   - JSON API: `https://localhost:5001/User/GetTemplates/11111111-1111-1111-1111-111111111111`
   - Direct API: `https://localhost:7263/api/template/user/11111111-1111-1111-1111-111111111111`

### Automated Testing

Unit tests exist in:
- `Template/Tests/Template.Tests/Controllers/TemplateControllerTests.cs`
  - `GetByUserId_ShouldReturnOkWithTemplates_WhenUserExists`
  - `GetByUserId_ShouldReturnNotFound_WhenUserDoesNotExist`

## 🎨 UI Features

The User Details page now includes:

- **User Information Card**: Displays user ID and name
- **Templates Section**: 
  - Dynamic table with template data
  - Loading spinner while fetching
  - "No templates" message when empty
  - Refresh button to reload data
  - Download JSON button (exports to file)
  - Download CSV button (exports to spreadsheet)

## 🔐 Security & CORS

- CORS is enabled on the Template API
- Environment-based configuration:
  - **Development**: Allows any origin
  - **Production**: Configurable allowed origins
- XSS protection via `textContent` in TypeScript
- CSV export uses RFC 4180 compliant escaping

## 📝 Code Comments

The method now includes comprehensive XML documentation:

```csharp
/// <summary>
/// Retrieves all templates associated with a specific user.
/// This method calls the Template microservice API endpoint: GET /api/template/user/{userId}
/// </summary>
/// <param name="userId">The unique identifier of the user</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>A collection of templates associated with the user</returns>
Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
```

## 🎓 Learning Resources

- **For Backend Developers**: See [TEMPLATE_SERVICE_USAGE.md](TEMPLATE_SERVICE_USAGE.md)
- **For Frontend Developers**: Check `TypeScript/userDetails.ts` and `Views/User/Details.cshtml`
- **For API Integration**: See [GETBYUSERID_EXAMPLES.md](GETBYUSERID_EXAMPLES.md)

## 🔗 Related Files

```
BaseWebApplication/WebApp/
├── Services/
│   ├── ITemplateService.cs          ← Interface definition
│   └── Imp/TemplateService.cs       ← Implementation
├── Controllers/
│   └── UserController.cs            ← GetTemplates action
├── Views/User/
│   └── Details.cshtml               ← UI with templates table
├── TypeScript/
│   └── userDetails.ts               ← Client-side logic
└── wwwroot/js/
    └── userDetails.js               ← Compiled TypeScript

Template/WebApiTemplate/
├── Controllers/
│   └── TemplateController.cs        ← API endpoint
└── Program.cs                       ← CORS configuration
```

## ❓ FAQ

**Q: Why isn't GetByUserIdAsync used in the UserController Details action?**
A: The Details action returns a view with the user data. The templates are loaded client-side via TypeScript for a better user experience (async loading, no page refresh needed for downloads).

**Q: When should I use the server-side GetTemplates endpoint vs client-side fetch?**
A: Use server-side when you need server-side processing (PDF generation, auth checks). Use client-side for dynamic UI updates and downloads.

**Q: Can I use this for other entities?**
A: Yes! The pattern can be replicated for other relationships (e.g., `GetTopicsByUserId`, `GetCommentsByUserId`).

## 📧 Support

For questions or issues:
1. Check the documentation files linked above
2. Review the code examples in this directory
3. Check existing tests in `Template/Tests/`

---

**Last Updated**: 2026-02-01
**Version**: 1.0
