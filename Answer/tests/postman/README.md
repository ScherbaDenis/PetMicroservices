# Postman Collection for Answer API

This directory contains Postman collections and environments for testing the Answer API.

## Files

- **Answer_API_Tests.postman_collection.json** - Main test collection with all API endpoints
- **Answer_API_Development.postman_environment.json** - Development environment configuration

## Features

### Test Coverage

The collection includes comprehensive tests for:

1. **Users API** (5 endpoints)
   - Create User
   - Get All Users
   - Get User by ID
   - Update User
   - Delete User

2. **Questions API** (5 endpoints)
   - Create Question
   - Get All Questions
   - Get Question by ID
   - Update Question
   - Delete Question

3. **Templates API** (5 endpoints)
   - Create Template
   - Get All Templates
   - Get Template by ID
   - Update Template
   - Delete Template

4. **Answers API** (5 endpoints + setup)
   - Setup test data (User, Question, Template)
   - Create Answer
   - Get All Answers
   - Get Answer by ID
   - Update Answer
   - Delete Answer

5. **Health Check**
   - Root endpoint verification

### Test Features

- **Automated Test Scripts**: Each request includes test scripts that validate:
  - HTTP status codes
  - Response structure
  - Required fields
  - Data integrity

- **Environment Variables**: Automatically captures and reuses IDs:
  - `userId`, `questionId`, `templateId`, `answerId`
  - `answerUserId`, `answerQuestionId`, `answerTemplateId` (for answer tests)

- **Pre-request Scripts**: 
  - Generate unique names for test data
  - Set up dependent data

## Import Instructions

### Import Collection

1. Open Postman
2. Click **Import** button
3. Select `Answer_API_Tests.postman_collection.json`
4. Click **Import**

### Import Environment

1. Click the environment dropdown (top right)
2. Click **Import**
3. Select `Answer_API_Development.postman_environment.json`
4. Click **Import**
5. Select "Answer API - Development" from the environment dropdown

## Usage

### Running Individual Requests

1. Select the "Answer API - Development" environment
2. Navigate to any request in the collection
3. Click **Send**
4. View the test results in the **Test Results** tab

### Running the Entire Collection

1. Click the collection name "Answer API Tests"
2. Click **Run** button
3. Select which folders/requests to run
4. Click **Run Answer API Tests**
5. View the test runner with all results

### Running with Newman (CLI)

Install Newman:
```bash
npm install -g newman
```

Run the collection:
```bash
newman run Answer_API_Tests.postman_collection.json \
  -e Answer_API_Development.postman_environment.json
```

Run with HTML report:
```bash
newman run Answer_API_Tests.postman_collection.json \
  -e Answer_API_Development.postman_environment.json \
  -r htmlextra \
  --reporter-htmlextra-export report.html
```

## Test Execution Order

For best results, run tests in this order:

1. **Health Check** - Verify API is running
2. **Users** - Create user data
3. **Questions** - Create question data
4. **Templates** - Create template data
5. **Answers** - Test complete workflow with all entities

Or use the **Answers → Setup** folder which automatically creates required test data.

## Customization

### Change Base URL

Update the `baseUrl` in the environment file or in the collection variables:
- Development: `http://localhost:5136`
- Production: Update to your production URL

### Add Custom Tests

Each request supports custom test scripts. Example:

```javascript
pm.test("Custom validation", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.name).to.have.lengthOf.at.least(3);
});
```

### Add Pre-request Scripts

Example pre-request script to generate test data:

```javascript
pm.variables.set("timestamp", Date.now());
pm.variables.set("randomId", pm.variables.replaceIn('{{$randomUUID}}'));
```

## Troubleshooting

### API Not Responding

1. Verify the API is running:
   ```bash
   cd src/Answer.Api
   dotnet run
   ```

2. Check the URL in the environment matches where the API is running

3. Ensure no firewall is blocking localhost:5136

### Tests Failing

1. Check the **Test Results** tab for specific error messages
2. Verify the response body in the **Body** tab
3. Ensure environment variables are set correctly
4. Try running requests individually to isolate the issue

### Environment Variables Not Set

Some tests depend on previous tests setting environment variables. Run tests in order:
1. Create operations first (to set IDs)
2. Then Get/Update/Delete operations (which use the IDs)

## CI/CD Integration

### GitHub Actions Example

```yaml
name: API Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '9.0.x'
      
      - name: Start API
        run: |
          cd src/Answer.Api
          dotnet run &
          sleep 10
      
      - name: Install Newman
        run: npm install -g newman
      
      - name: Run Postman Tests
        run: |
          cd postman
          newman run Answer_API_Tests.postman_collection.json \
            -e Answer_API_Development.postman_environment.json
```

## Support

For issues or questions:
- Check the API documentation in the main README.md
- Review the API endpoints in the Swagger/gRPC reflection
- Open an issue in the repository
