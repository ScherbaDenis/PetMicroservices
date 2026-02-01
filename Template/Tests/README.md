# WebApiTemplate Postman Tests

This folder contains Postman collection and environment files for testing the WebApiTemplate microservice.

## Files

- `WebApiTemplate.postman_collection.json` - Complete test collection with CRUD operations for all endpoints
- `WebApiTemplate.postman_environment.json` - Environment variables for local development

## Getting Started

### Prerequisites

- [Postman](https://www.postman.com/downloads/) installed on your machine
- WebApiTemplate service running locally

### Setup Instructions

1. **Start the WebApiTemplate service:**
   ```bash
   cd Template/WebApiTemplate
   dotnet run
   ```
   By default, the service runs on `http://localhost:5000` (or check the console output for the actual port).

2. **Import the Postman Collection:**
   - Open Postman
   - Click on "Import" button
   - Select `WebApiTemplate.postman_collection.json`
   - Click "Import"

3. **Import the Postman Environment:**
   - Click on "Import" button again
   - Select `WebApiTemplate.postman_environment.json`
   - Click "Import"

4. **Select the Environment:**
   - In the top-right corner of Postman, select "WebApiTemplate - Local Development" from the environment dropdown

5. **Update the Base URL (if needed):**
   - If your service is running on a different port, update the `baseUrl` variable in the environment
   - Click on the environment name and edit the `baseUrl` value

## Running Tests

### Run All Tests
1. Select the "WebApiTemplate Tests" collection
2. Click on the "..." menu next to the collection name
3. Select "Run collection"
4. Click "Run WebApiTemplate Tests"

### Run Individual Test Groups
You can run tests for specific endpoints:
- **Template Endpoints** - CRUD operations for templates
- **User Endpoints** - CRUD operations for users
- **Topic Endpoints** - CRUD operations for topics
- **Tag Endpoints** - CRUD operations for tags
- **WeatherForecast Endpoint** - Get weather forecast data

### Test Execution Order
For best results, run tests in the following order within each endpoint group:
1. Get All (to populate environment variables)
2. Get by ID (uses ID from step 1)
3. Create (creates a new entity and saves its ID)
4. Update (updates the created entity)
5. Delete (deletes the created entity)
6. Get Non-Existent (404 test)

## Test Coverage

The collection includes comprehensive tests for:

### Template Endpoints (`/api/template`)
- ✅ GET all templates
- ✅ GET template by ID
- ✅ POST create template
- ✅ PUT update template
- ✅ DELETE template
- ✅ GET non-existent template (404 test)

### User Endpoints (`/api/user`)
- ✅ GET all users
- ✅ GET user by ID
- ✅ GET user templates (`/api/user/{id}/templates`)
- ✅ POST create user
- ✅ PUT update user
- ✅ DELETE user
- ✅ GET non-existent user (404 test)

### Topic Endpoints (`/api/topic`)
- ✅ GET all topics
- ✅ GET topic by ID
- ✅ POST create topic
- ✅ PUT update topic
- ✅ DELETE topic
- ✅ GET non-existent topic (404 test)

### Tag Endpoints (`/api/tag`)
- ✅ GET all tags
- ✅ GET tag by ID
- ✅ POST create tag
- ✅ PUT update tag
- ✅ DELETE tag
- ✅ GET non-existent tag (404 test)

### WeatherForecast Endpoint (`/WeatherForecast`)
- ✅ GET weather forecast

## Environment Variables

The environment includes the following variables:

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `baseUrl` | Base URL of the API | `http://localhost:5000` |
| `templateId` | ID of an existing template | `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa` |
| `userId` | ID of an existing user | `11111111-1111-1111-1111-111111111111` |
| `topicId` | ID of an existing topic | `1` |
| `tagId` | ID of an existing tag | `1` |
| `createdTemplateId` | ID of template created during tests | (populated automatically) |
| `createdUserId` | ID of user created during tests | (populated automatically) |
| `createdTopicId` | ID of topic created during tests | (populated automatically) |
| `createdTagId` | ID of tag created during tests | (populated automatically) |

## Test Assertions

Each request includes automatic test assertions that verify:
- ✅ Correct HTTP status codes
- ✅ Response structure and required properties
- ✅ Data integrity (IDs match, data persists correctly)
- ✅ Response time performance (< 2000ms)
- ✅ Error handling (404 for non-existent resources)

## Troubleshooting

### Connection Errors
- Ensure the WebApiTemplate service is running
- Verify the port number matches the `baseUrl` in the environment
- Check if the database is properly configured and accessible

### Test Failures
- Run "Get All" requests first to populate environment variables with existing IDs
- Ensure the database has seed data (users, topics, tags, templates)
- Check the console output for detailed error messages

### Database Issues
- The service uses SQL Server by default
- Connection string is in `appsettings.json`
- In Development mode, the database is automatically seeded with initial data

## Notes

- Tests use Postman's dynamic variables (e.g., `{{$guid}}`) to generate unique IDs
- Environment variables are automatically updated during test execution
- The collection is designed to be idempotent - you can run it multiple times
- Some tests depend on previous tests for environment variable population
