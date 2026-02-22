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

// Configure CORS for gRPC-Web
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }
        else
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? new[] { "https://localhost:7200", "http://localhost:5000" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }
    });
});

// Add DbContext (skip in Testing environment - test factory provides its own)
if (!builder.Environment.IsEnvironment("Testing"))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AnswerDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Add Infrastructure services (repositories)
builder.Services.AddInfrastructure();

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("answer-service", false));
    x.AddConsumer<TemplateCreatedEventConsumer>();
    x.AddConsumer<UserCreatedEventConsumer>();
    x.AddConsumer<QuestionCreatedEventConsumer>();

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

// Enable CORS
app.UseCors("AllowAll");

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

// Apply database migrations on startup (skip in Testing environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AnswerDbContext>();
    dbContext.Database.Migrate();
}

app.Run();

// Make the Program class accessible to integration tests
public partial class Program { }
