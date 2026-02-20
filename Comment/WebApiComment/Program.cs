using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Comment.Domain.Repositories;
using Comment.Domain.Services;
using Comment.Service.Services;
using Comment.Domain.Models;
using CommentEntity = Comment.Domain.Models.Comment;
using MassTransit;
using WebApiComment.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//DbContext
builder.Services.AddControllers();// DbContext

// Only configure SQL Server DbContext if not in Testing environment
if (!builder.Environment.IsEnvironment("Testing"))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<CommentDbContext>(options =>
        options.UseSqlServer(connectionString)
               .UseSeeding((context, _) =>
               {
               // Only seed data in Development environment
               var env = builder.Environment;
               if (env.IsDevelopment())
               {
                   // Check if data already exists
                   if (!context.Set<Template>().Any())
                   {
                       // Seed Templates - using unique GUIDs for Comment microservice
                       var template1 = new Template
                       {
                           Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                           Title = "Customer Feedback Template"
                       };
                       var template2 = new Template
                       {
                           Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                           Title = "Product Review Template"
                       };
                       var template3 = new Template
                       {
                           Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                           Title = "Support Ticket Template"
                       };

                       context.Set<Template>().AddRange(template1, template2, template3);
                       context.SaveChanges();

                       // Seed Comments - initial sample data for demonstration
                       context.Set<CommentEntity>().AddRange(
                           new CommentEntity
                           {
                               Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                               Text = "Great product! Highly recommended.",
                               Template = template1
                           },
                           new CommentEntity
                           {
                               Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                               Text = "The service was excellent and very helpful.",
                               Template = template1
                           },
                           new CommentEntity
                           {
                               Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                               Text = "Good quality, but a bit expensive.",
                               Template = template2
                           },
                           new CommentEntity
                           {
                               Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                               Text = "Fast delivery and good packaging.",
                               Template = template2
                           },
                           new CommentEntity
                           {
                               Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                               Text = "Issue resolved quickly by support team.",
                               Template = template3
                           }
                       );
                       context.SaveChanges();
                   }
               }
           })
           .UseAsyncSeeding(async (context, _, cancellationToken) =>
           {
               // Async seeding - runs after UseSeeding
               // Only seed data in Development environment
               var env = builder.Environment;
               if (env.IsDevelopment())
               {
                   // Ensure migrations are applied
                   await context.Database.MigrateAsync(cancellationToken);
               }
           }));
}

// Repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Services
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
