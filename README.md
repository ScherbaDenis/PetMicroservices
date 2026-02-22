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

4. **Answer Service** (`Answer.Api`) - Port 5054
   - Manages answers and questions
   - gRPC API with RabbitMQ integration
   - [Documentation](Answer/README.md)

5. **WebApp** (`BaseWebApplication/WebApp`) - Port 5177
   - MVC frontend that connects to backend services via the API Gateway
   - Includes TypeScript frontend compiled at build time

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

3. Start the Answer Service:
   ```bash
   cd Answer/src/Answer.Api
   dotnet run
   ```

4. Start the API Gateway:
   ```bash
   cd ApiGateway/ApiGateway
   dotnet run
   ```

5. Start the WebApp:
   ```bash
   cd BaseWebApplication/WebApp
   dotnet run
   # Runs on http://localhost:5177
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

## Docker

All services, including WebApp, can be started together with Docker Compose.

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and [Docker Compose](https://docs.docker.com/compose/install/) installed

### Environment Variables

Copy `.env.example` to `.env` and adjust the values if needed:

```bash
cp .env.example .env
```

The `.env` file is gitignored. Default values work out of the box if you omit this step.

| Variable | Default | Description |
|---|---|---|
| `SA_PASSWORD` | `YourStrong!Passw0rd` | SQL Server SA password |
| `RABBITMQ_USER` | `guest` | RabbitMQ username |
| `RABBITMQ_PASS` | `guest` | RabbitMQ password |

### Starting All Services

```bash
docker compose up --build
```

This starts all microservices, the API Gateway, SQL Server, RabbitMQ, and the WebApp frontend together.

| Service | URL |
|---|---|
| WebApp | http://localhost:5177 |
| API Gateway | http://localhost:5000 |
| Comment Service | http://localhost:5234 |
| Template Service | http://localhost:5100 |
| Answer Service | http://localhost:5136 |
| RabbitMQ Management | http://localhost:15672 |

### Building the WebApp Image Only

```bash
docker build -f BaseWebApplication/WebApp/Dockerfile -t webapp .
```

### Running the WebApp Container Standalone

```bash
docker run -p 5177:8080 \
  -e ApiEndpoints__UserService=http://api-gateway:8080/user \
  -e ApiEndpoints__TopicService=http://api-gateway:8080/topic \
  -e ApiEndpoints__TemplateService=http://api-gateway:8080/template \
  -e ApiEndpoints__QuestionService=http://api-gateway:8080/question \
  -e ApiEndpoints__TagService=http://api-gateway:8080/tag \
  -e ApiEndpoints__CommentService=http://api-gateway:8080/comment \
  -e ApiEndpoints__AnswerService=http://api-gateway:8080 \
  webapp
```

### Stopping All Services

```bash
docker compose down
```

To also remove the SQL Server data volume:

```bash
docker compose down -v
```

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
#### rabbitmq
docker run -d --name rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management
For detailed instructions, see [POSTMAN_TESTS_README.md](Comment/WebApiComment/POSTMAN_TESTS_README.md).
