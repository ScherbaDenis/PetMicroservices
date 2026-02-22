using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Template.DataAccess.MsSql;
using Template.DataAccess.MsSql.Repositories;
using Template.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // JsonPolymorphic attributes automatically handle discriminators in .NET 7+
    });

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
                ?? new[] { "https://localhost:5001" };

            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

builder.Services.AddTemplateDataAccess(builder.Configuration, builder.Environment);
builder.Services.AddTemplateServices();

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
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
    var dbContext = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();

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
