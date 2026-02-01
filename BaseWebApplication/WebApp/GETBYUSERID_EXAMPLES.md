# GetByUserIdAsync Usage Examples

## Quick Reference

### Method Flow

```
UserController (WebApp)
    ↓
ITemplateService.GetByUserIdAsync(userId)
    ↓
TemplateService.GetByUserIdAsync(userId)
    ↓
HttpClient → GET /api/template/user/{userId}
    ↓
Template Microservice API
    ↓
Returns: IEnumerable<TemplateDto>
```

## Example 1: Server-Side Controller Usage

**Scenario**: Get user templates as JSON API response

```csharp
// UserController.cs
[HttpGet]
public async Task<IActionResult> GetTemplates(Guid id, CancellationToken cancellationToken)
{
    var user = await _service.GetByIdAsync(id, cancellationToken);
    if (user == null) return NotFound();

    // Using GetByUserIdAsync
    var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
    
    return Json(new { 
        userId = user.Id, 
        userName = user.Name, 
        templates = templates
    });
}
```

**Test URL**: 
```
GET /User/GetTemplates/11111111-1111-1111-1111-111111111111
```

**Expected Response**:
```json
{
  "userId": "11111111-1111-1111-1111-111111111111",
  "userName": "John Doe",
  "templates": [
    {
      "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "title": "Customer Feedback Survey",
      "description": "A template for collecting customer feedback",
      "owner": {
        "id": "11111111-1111-1111-1111-111111111111",
        "name": "John Doe"
      }
    }
  ],
  "count": 1
}
```

## Example 2: Client-Side TypeScript Usage

**Scenario**: Fetch and display user templates in the UI

```typescript
// TypeScript/userDetails.ts
class UserTemplateManager {
    async fetchUserTemplates(): Promise<TemplateDto[]> {
        const response = await fetch(
            `${this.apiBaseUrl}/user/${this.userId}`,
            {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                mode: 'cors'
            }
        );
        return await response.json();
    }

    async displayTemplates(): Promise<void> {
        const templates = await this.fetchUserTemplates();
        // Render templates in table
        templates.forEach(template => {
            // Add row to table
        });
    }
}
```

**Usage in View**:
```html
<!-- Views/User/Details.cshtml -->
<div id="userId" data-user-id="@Model.Id"></div>
<table id="templatesTable">
    <tbody id="templatesTableBody"></tbody>
</table>

<script src="~/js/userDetails.js"></script>
```

## Example 3: Direct API Call

**Using cURL**:
```bash
curl -X GET "https://localhost:7263/api/template/user/11111111-1111-1111-1111-111111111111" \
     -H "accept: application/json"
```

**Using PowerShell**:
```powershell
Invoke-RestMethod -Uri "https://localhost:7263/api/template/user/11111111-1111-1111-1111-111111111111" `
                  -Method GET
```

**Using JavaScript fetch**:
```javascript
fetch('/proxy/template/user/11111111-1111-1111-1111-111111111111')
    .then(response => response.json())
    .then(templates => console.log(templates));
```

## Example 4: Passing Templates to View

**Scenario**: Load templates server-side and pass to view

```csharp
public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
{
    var user = await _service.GetByIdAsync(id, cancellationToken);
    if (user == null) return NotFound();
    
    // Fetch user templates
    var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
    
    // Pass to view via ViewBag
    ViewBag.Templates = templates;
    ViewBag.TemplateCount = templates.Count();
    
    return View(user);
}
```

**In the View**:
```html
@{
    var templates = ViewBag.Templates as IEnumerable<TemplateDto>;
}

<h3>Templates (@ViewBag.TemplateCount)</h3>
<ul>
    @foreach (var template in templates)
    {
        <li>@template.Title - @template.Description</li>
    }
</ul>
```

## Example 5: Export User Templates

**Scenario**: Generate downloadable report of user templates

```csharp
[HttpGet]
public async Task<IActionResult> ExportTemplates(Guid id, CancellationToken cancellationToken)
{
    var user = await _service.GetByIdAsync(id, cancellationToken);
    if (user == null) return NotFound();

    var templates = await _templateService.GetByUserIdAsync(id, cancellationToken);
    
    // Generate CSV
    var csv = new StringBuilder();
    csv.AppendLine("Title,Description,Topic");
    
    foreach (var template in templates)
    {
        csv.AppendLine($"\"{template.Title}\",\"{template.Description}\",\"{template.Topic?.Name}\"");
    }
    
    return File(
        Encoding.UTF8.GetBytes(csv.ToString()),
        "text/csv",
        $"user-{user.Name}-templates.csv"
    );
}
```

## Testing with Seeded Data

The Template database is seeded with test data in development:

**Users**:
- User 1: `11111111-1111-1111-1111-111111111111` (John Doe)
- User 2: `22222222-2222-2222-2222-222222222222` (Jane Smith)

**Templates**:
- Template 1: `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa` (Customer Feedback Survey)
- Template 2: `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb` (Employee Onboarding Checklist)

**Test URLs**:
```
GET /User/GetTemplates/11111111-1111-1111-1111-111111111111
GET /User/GetTemplates/22222222-2222-2222-2222-222222222222
GET /User/Details/11111111-1111-1111-1111-111111111111
```

## Error Handling

```csharp
try
{
    var templates = await _templateService.GetByUserIdAsync(userId, cancellationToken);
    
    if (!templates.Any())
    {
        return Ok(new { message = "User has no templates", templates = Array.Empty<TemplateDto>() });
    }
    
    return Ok(templates);
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
{
    _logger.LogWarning("User {UserId} not found", userId);
    return NotFound(new { error = "User not found" });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error fetching templates for user {UserId}", userId);
    return StatusCode(500, new { error = "Failed to retrieve templates" });
}
```

## Performance Considerations

- The method makes an HTTP call to the Template microservice
- Consider caching frequently accessed template lists
- Use async/await to avoid blocking
- Implement timeout policies for resilience

```csharp
// Example with caching
var cacheKey = $"user-templates-{userId}";
if (!_cache.TryGetValue(cacheKey, out IEnumerable<TemplateDto> templates))
{
    templates = await _templateService.GetByUserIdAsync(userId, cancellationToken);
    _cache.Set(cacheKey, templates, TimeSpan.FromMinutes(5));
}
return templates;
```

## Related Documentation

- [Main Usage Guide](TEMPLATE_SERVICE_USAGE.md)
- [User Details View](Views/User/Details.cshtml)
- [TypeScript Implementation](TypeScript/userDetails.ts)
- [Template API Controller](../../Template/WebApiTemplate/Controllers/TemplateController.cs)
