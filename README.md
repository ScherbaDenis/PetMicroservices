# PetMicroservices
Test project from learn Microservices.

## Architecture

This project demonstrates a microservices architecture with the following components:

### Services

1. **API Gateway** (`ApiGateway`) - Port 5000
   - Single entry point for all client requests
   - Routes requests to appropriate microservices
   - Built with Ocelot
   - [Documentation](ApiGateway/README.md)

2. **Comment Service** (`WebApiComment`) - Port 5234
   - Manages comments and their templates
   - REST API for comment operations

3. **Template Service** (`WebApiTemplate`) - Port 5100
   - Manages templates, users, topics, and tags
   - REST API for template-related operations

### Getting Started

#### Running the Services

1. Start the Comment Service:
   ```bash
   cd Comment/WebApiComment
   dotnet run
   ```

2. Start the Template Service:
   ```bash
   cd Template/WebApiTemplate
   dotnet run
   ```

3. Start the API Gateway:
   ```bash
   cd ApiGateway/ApiGateway
   dotnet run
   ```

#### Using the API Gateway

Once all services are running, access them through the API Gateway at `http://localhost:5000`. For example:

```bash
# Get all comments
curl http://localhost:5000/comment

# Get all templates
curl http://localhost:5000/template

# Get all users
curl http://localhost:5000/user
```

See the [API Gateway documentation](ApiGateway/README.md) for complete route information.

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

#### RabbitMQ

To run RabbitMQ for message queuing:
```bash
docker run -d --name rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
```

For detailed instructions, see [POSTMAN_TESTS_README.md](Comment/WebApiComment/POSTMAN_TESTS_README.md).
