# PetMicroservices
Test project from learn Microservices.

## Database Setup

To generate and set up the databases for the microservices, use the provided database generation scripts:

- **Comment Microservice**: Run `./Comment/generate-db.sh` (Linux/macOS) or `Comment\generate-db.bat` (Windows)
- **Template Microservice**: Run `./Template/generate-db.sh` (Linux/macOS) or `Template\generate-db.bat` (Windows)

For detailed instructions and manual database operations, see [DATABASE_GENERATION.md](DATABASE_GENERATION.md).

## Testing

### Postman Tests

The WebApiComment microservice includes comprehensive Postman test collections for API testing:

- **Location**: `Comment/WebApiComment/`
- **Files**:
  - `WebApiComment.postman_collection.json` - Complete test collection with 14 automated test requests
  - `WebApiComment.postman_environment.json` - Environment configuration for local development
  - `POSTMAN_TESTS_README.md` - Detailed documentation and usage instructions

#### Quick Start

1. Import the collection and environment files into Postman
2. Start the WebApiComment service: `cd Comment/WebApiComment && dotnet run`
3. Run the tests using Postman's Collection Runner or via Newman CLI:
   ```bash
   newman run Comment/WebApiComment/WebApiComment.postman_collection.json \
     -e Comment/WebApiComment/WebApiComment.postman_environment.json
   ```

For detailed instructions, see [POSTMAN_TESTS_README.md](Comment/WebApiComment/POSTMAN_TESTS_README.md).
