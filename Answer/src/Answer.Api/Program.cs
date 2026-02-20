using Answer.Api.Consumers;
using Answer.Api.Services;
using Answer.Infrastructure;
using Answer.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TemplateCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
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

// Apply database migrations when using SQL Server
var useInMemory = app.Configuration.GetValue<bool>("UseInMemoryDatabase", true);
if (!useInMemory && !app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AnswerDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

// Make the Program class accessible to integration tests
public partial class Program { }
