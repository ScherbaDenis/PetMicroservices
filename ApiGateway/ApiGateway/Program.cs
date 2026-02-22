using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiGatewayCorsPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // In development, allow any origin
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // In production, restrict to specific origins
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? new[] { "https://localhost:7200", "https://localhost:5177" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Enable CORS
app.UseCors("ApiGatewayCorsPolicy");

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
