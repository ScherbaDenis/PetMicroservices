using Comment.DataAccess.MsSql;
using Comment.DataAccess.MsSql.Repositories;
using Comment.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using WebApiComment.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        // In development, allow any origin. In production, should be restricted to specific domains
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // In production, restrict to specific origins from configuration
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? new[] { "https://localhost:7200", "http://localhost:5000" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

builder.Services.AddCommentDataAccess(builder.Configuration, builder.Environment);
builder.Services.AddCommentServices();

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("comment-service", false));
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

// Ensure database is created and migrations are applied
if (!builder.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CommentDbContext>();

    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.

// Only use HTTPS redirection if not in Testing environment
if (!builder.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
