# API Gateway

This API Gateway is built using [Ocelot](https://github.com/ThreeMammals/Ocelot), which provides a simple and elegant way to route requests to multiple microservices.

## Overview

The API Gateway acts as a single entry point for all client requests, routing them to the appropriate microservices:
- **Comment Service** (http://localhost:5234)
- **Template Service** (http://localhost:5100)

## Configuration

The gateway is configured via `ocelot.json` file, which defines:
- Route mappings from gateway endpoints to downstream services
- HTTP methods allowed for each route
- Host and port information for each service

## Running the API Gateway

### Prerequisites
Ensure the following microservices are running:
1. Comment Service on port 5234
2. Template Service on port 5100

### Start the Gateway
```bash
cd ApiGateway/ApiGateway
dotnet run
```

The gateway will start on **http://localhost:5000**

## API Routes

### Comment Service Routes

#### Comments
- `GET /comment` - Get all comments → `http://localhost:5234/api/comment`
- `POST /comment` - Create a comment → `http://localhost:5234/api/comment`
- `GET /comment/{id}` - Get comment by ID → `http://localhost:5234/api/comment/{id}`
- `PUT /comment/{id}` - Update comment → `http://localhost:5234/api/comment/{id}`
- `DELETE /comment/{id}` - Delete comment → `http://localhost:5234/api/comment/{id}`
- `GET /comment/template/{templateId}` - Get comments by template → `http://localhost:5234/api/comment/template/{templateId}`

#### Comment Templates
- `GET /comment-template` - Get all templates from Comment service → `http://localhost:5234/api/template`
- `POST /comment-template` - Create template → `http://localhost:5234/api/template`
- `GET /comment-template/{id}` - Get template by ID → `http://localhost:5234/api/template/{id}`
- `PUT /comment-template/{id}` - Update template → `http://localhost:5234/api/template/{id}`
- `DELETE /comment-template/{id}` - Delete template → `http://localhost:5234/api/template/{id}`

### Template Service Routes

#### Templates
- `GET /template` - Get all templates → `http://localhost:5100/api/template`
- `POST /template` - Create template → `http://localhost:5100/api/template`
- `GET /template/{id}` - Get template by ID → `http://localhost:5100/api/template/{id}`
- `PUT /template/{id}` - Update template → `http://localhost:5100/api/template/{id}`
- `DELETE /template/{id}` - Delete template → `http://localhost:5100/api/template/{id}`

#### Users
- `GET /user` - Get all users → `http://localhost:5100/api/user`
- `POST /user` - Create user → `http://localhost:5100/api/user`
- `GET /user/{id}` - Get user by ID → `http://localhost:5100/api/user/{id}`
- `PUT /user/{id}` - Update user → `http://localhost:5100/api/user/{id}`
- `DELETE /user/{id}` - Delete user → `http://localhost:5100/api/user/{id}`

#### Topics
- `GET /topic` - Get all topics → `http://localhost:5100/api/topic`
- `POST /topic` - Create topic → `http://localhost:5100/api/topic`
- `GET /topic/{id}` - Get topic by ID → `http://localhost:5100/api/topic/{id}`
- `PUT /topic/{id}` - Update topic → `http://localhost:5100/api/topic/{id}`
- `DELETE /topic/{id}` - Delete topic → `http://localhost:5100/api/topic/{id}`

#### Tags
- `GET /tag` - Get all tags → `http://localhost:5100/api/tag`
- `POST /tag` - Create tag → `http://localhost:5100/api/tag`
- `GET /tag/{id}` - Get tag by ID → `http://localhost:5100/api/tag/{id}`
- `PUT /tag/{id}` - Update tag → `http://localhost:5100/api/tag/{id}`
- `DELETE /tag/{id}` - Delete tag → `http://localhost:5100/api/tag/{id}`

### Answer Service Routes

#### Answers
- `GET /answer` - Get all answers → `http://localhost:5136/api/answers`
- `POST /answer` - Create answer → `http://localhost:5136/api/answers`
- `GET /answer/{id}` - Get answer by ID → `http://localhost:5136/api/answers/{id}`
- `PUT /answer/{id}` - Update answer → `http://localhost:5136/api/answers/{id}`
- `DELETE /answer/{id}` - Delete answer → `http://localhost:5136/api/answers/{id}`

#### Answer Users
- `GET /answer-user` - Get all users from Answer service → `http://localhost:5136/api/users`
- `POST /answer-user` - Create user → `http://localhost:5136/api/users`
- `GET /answer-user/{id}` - Get user by ID → `http://localhost:5136/api/users/{id}`
- `PUT /answer-user/{id}` - Update user → `http://localhost:5136/api/users/{id}`
- `DELETE /answer-user/{id}` - Delete user → `http://localhost:5136/api/users/{id}`

#### Questions
- `GET /question` - Get all questions → `http://localhost:5136/api/questions`
- `POST /question` - Create question → `http://localhost:5136/api/questions`
- `GET /question/{id}` - Get question by ID → `http://localhost:5136/api/questions/{id}`
- `PUT /question/{id}` - Update question → `http://localhost:5136/api/questions/{id}`
- `DELETE /question/{id}` - Delete question → `http://localhost:5136/api/questions/{id}`

#### Answer Templates
- `GET /answer-template` - Get all templates from Answer service → `http://localhost:5136/api/templates`
- `POST /answer-template` - Create template → `http://localhost:5136/api/templates`
- `GET /answer-template/{id}` - Get template by ID → `http://localhost:5136/api/templates/{id}`
- `PUT /answer-template/{id}` - Update template → `http://localhost:5136/api/templates/{id}`
- `DELETE /answer-template/{id}` - Delete template → `http://localhost:5136/api/templates/{id}`

## Example Usage

### Using curl

```bash
# Get all comments through the gateway
curl http://localhost:5000/comment

# Get all templates from Template service
curl http://localhost:5000/template

# Get all users
curl http://localhost:5000/user

# Create a new comment
curl -X POST http://localhost:5000/comment \
  -H "Content-Type: application/json" \
  -d '{"text": "Great service!", "templateId": "some-guid"}'
```

## Architecture Benefits

Using an API Gateway provides several advantages:
1. **Single Entry Point** - Clients only need to know one URL
2. **Service Abstraction** - Hide internal service structure from clients
3. **Simplified Client Code** - Reduce the number of service endpoints clients need to track
4. **Easy to Add Features** - Can add authentication, rate limiting, logging, etc. at gateway level
5. **Load Balancing** - Can distribute requests across multiple instances of the same service

## Configuration Changes

To add new routes or modify existing ones, edit the `ocelot.json` file and restart the gateway.

### Route Configuration Structure

```json
{
  "DownstreamPathTemplate": "/api/endpoint",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "localhost",
      "Port": 5234
    }
  ],
  "UpstreamPathTemplate": "/gateway-endpoint",
  "UpstreamHttpMethod": [ "GET", "POST" ]
}
```

## Technology Stack

- **.NET 8.0** - Runtime framework
- **Ocelot 24.1.0** - API Gateway library
- **ASP.NET Core** - Web framework
