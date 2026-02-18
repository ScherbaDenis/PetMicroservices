# Answer Integration in WebApp

## Overview

This integration adds full CRUD support for Answers in the WebApp, connecting to the Answer microservice API.

## What Was Added

### 1. Data Transfer Objects (DTOs)
**Location**: `BaseWebApplication/WebApp/Services/DTOs/AnswerDto.cs`

- `AnswerDto` - Represents a complete answer with all related data
- `CreateAnswerDto` - Used for creating new answers
- `UpdateAnswerDto` - Used for updating existing answers
- `AnswerType` enum - Defines answer types (SingleLineString, MultiLineText, PositiveInteger, Checkbox, Boolean)

### 2. Service Layer
**Location**: `BaseWebApplication/WebApp/Services/`

- `IAnswerService.cs` - Service interface defining CRUD operations
- `Services/Imp/AnswerService.cs` - HTTP client implementation that communicates with the Answer API

**Features**:
- Uses camelCase JSON serialization for compatibility with gRPC-JSON transcoding
- Case-insensitive deserialization for flexibility
- Proper enum handling with string converter
- Configured to use Answer API at `https://localhost:7065/api/answers`

### 3. Controller
**Location**: `BaseWebApplication/WebApp/Controllers/AnswerController.cs`

Implements standard CRUD operations:
- `Index` - List all answers
- `Details` - View answer details
- `Create` - Create new answer
- `Edit` - Update existing answer
- `Delete` - Delete answer

**Features**:
- Populates dropdown lists for Users, Questions, and Templates
- Proper validation and error handling
- Follows the same pattern as other controllers (Comment, Question, etc.)

### 4. Views
**Location**: `BaseWebApplication/WebApp/Views/Answer/`

- `Index.cshtml` - List all answers in a table
- `Create.cshtml` - Form to create a new answer
- `Edit.cshtml` - Form to edit an existing answer
- `Details.cshtml` - Display answer details
- `Delete.cshtml` - Confirmation page for deletion

### 5. Configuration

**appsettings.json Updates**:
```json
{
  "ApiEndpoints": {
    "AnswerService": "https://localhost:7065/api/answers"
  },
  "ReverseProxy": {
    "Routes": {
      "answer-route": {
        "ClusterId": "answer-cluster",
        "Match": {
          "Path": "/proxy/answer/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "answer-cluster": {
        "Destinations": {
          "answer-service": {
            "Address": "https://localhost:7065"
          }
        }
      }
    }
  }
}
```

**Program.cs Updates**:
- Registered `IAnswerService` with dependency injection
- Added HTTP client for AnswerService

**Navigation Updates**:
- Added "Answers" link to the main navigation menu

## How to Test

### Prerequisites
1. Ensure the Answer API is running on `https://localhost:7065`
   ```bash
   cd Answer/src/Answer.Api
   dotnet run
   ```

2. Ensure the Template API is running (for users, questions, and templates)
   ```bash
   cd Template/WebApiTemplate
   dotnet run
   ```

3. Ensure the WebApp is running
   ```bash
   cd BaseWebApplication/WebApp
   dotnet run
   ```

### Testing Steps

1. **Navigate to Answers**
   - Open browser to the WebApp (typically https://localhost:[port])
   - Click on "Answers" in the navigation menu

2. **Create an Answer**
   - Click "Create New"
   - Select a User, Question, and Template from the dropdowns
   - Choose an Answer Type
   - Enter an Answer Value
   - Click "Create"

3. **View Answer Details**
   - From the Answers list, click "Details" on any answer
   - Verify all answer information is displayed correctly

4. **Edit an Answer**
   - From the Answers list, click "Edit" on any answer
   - Modify the Answer Type or Answer Value
   - Click "Save"
   - Verify the changes are reflected

5. **Delete an Answer**
   - From the Answers list, click "Delete" on any answer
   - Confirm the deletion
   - Verify the answer is removed from the list

### API Compatibility Notes

The Answer API uses gRPC with JSON transcoding:
- **Endpoint**: `/api/answers`
- **Request Format**: JSON with camelCase properties
- **Response Format**: JSON with camelCase properties
- **Enum Format**: String values (e.g., "SingleLineString", "MultiLineText")

The AnswerService is configured to handle this properly:
- `PropertyNamingPolicy = JsonNamingPolicy.CamelCase` for serialization
- `PropertyNameCaseInsensitive = true` for deserialization
- `JsonStringEnumConverter` for enum handling

## Integration Architecture

```
WebApp (ASP.NET Core MVC)
  ├── AnswerController
  │     └── IAnswerService
  │           └── AnswerService (HTTP Client)
  │                 └── Answer API (gRPC + REST)
  │
  └── Views/Answer/*.cshtml
```

## Future Enhancements

Potential improvements:
1. Add filtering and sorting to the Answers list
2. Add validation for answer values based on answer type
3. Add pagination for large answer lists
4. Add search functionality
5. Add ability to filter answers by user, question, or template
6. Implement caching for frequently accessed data
7. Add comprehensive integration tests for the Answer functionality
