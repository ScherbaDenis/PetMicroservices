using Answer.Api.Services;
using Answer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

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
