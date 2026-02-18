using Answer.Api.Services;
using Answer.Infrastructure;
using Answer.Api.Protos;
using Grpc.Net.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Register gRPC clients for internal service calls
// These clients call the UserService, QuestionService, and TemplateService
// implemented within this same application
builder.Services.AddSingleton(sp =>
{
    // Get the application URL from configuration or use a default
    var serverUrl = builder.Configuration.GetValue<string>("GrpcServerUrl") ?? "http://localhost:5136";
    var channel = GrpcChannel.ForAddress(serverUrl);
    return new UserService.UserServiceClient(channel);
});

builder.Services.AddSingleton(sp =>
{
    var serverUrl = builder.Configuration.GetValue<string>("GrpcServerUrl") ?? "http://localhost:5136";
    var channel = GrpcChannel.ForAddress(serverUrl);
    return new QuestionService.QuestionServiceClient(channel);
});

builder.Services.AddSingleton(sp =>
{
    var serverUrl = builder.Configuration.GetValue<string>("GrpcServerUrl") ?? "http://localhost:5136";
    var channel = GrpcChannel.ForAddress(serverUrl);
    return new TemplateService.TemplateServiceClient(channel);
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Map gRPC services
app.MapGrpcService<UserServiceImpl>().EnableGrpcWeb();
app.MapGrpcService<QuestionServiceImpl>().EnableGrpcWeb();
app.MapGrpcService<TemplateServiceImpl>().EnableGrpcWeb();
app.MapGrpcService<AnswerServiceImpl>().EnableGrpcWeb();

// Enable gRPC reflection for development
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Answer API - gRPC with REST support");

app.Run();

// Make the Program class accessible to integration tests
public partial class Program { }
