using Microsoft.EntityFrameworkCore;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Repository;
using Template.Domain.Services;
using Template.Service.Services;
using Template.Domain.Model;

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
                ?? new[] { "https://localhost:5001" };
            
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// DbContext
if (!builder.Environment.IsEnvironment("Testing"))
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<TemplateDbContext>(options =>
        options.UseSqlServer(connectionString)
               .UseSeeding((context, _) =>
               {
                   // Only seed data in Development environment
                   var env = builder.Environment;
                   if (env.IsDevelopment())
                   {
                       // Check if data already exists
                       if (!context.Set<User>().Any())
                       {
                           // Seed Users
                           var user1 = new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "John Doe" };
                           var user2 = new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Jane Smith" };
                           context.Set<User>().AddRange(user1, user2);
                           context.SaveChanges();
                       }

                       if (!context.Set<Topic>().Any())
                       {
                           // Seed Topics
                           context.Set<Topic>().AddRange(
                               new Topic { Name = "Technology" },
                               new Topic { Name = "Science" },
                               new Topic { Name = "Education" }
                           );
                           context.SaveChanges();
                       }

                       if (!context.Set<Tag>().Any())
                       {
                           // Seed Tags
                           context.Set<Tag>().AddRange(
                               new Tag { Name = "Programming" },
                               new Tag { Name = "Database" },
                               new Tag { Name = "Web Development" },
                               new Tag { Name = "Machine Learning" }
                           );
                           context.SaveChanges();
                       }

                       if (!context.Set<Template.Domain.Model.Template>().Any())
                       {
                           // Seed Templates
                           context.Set<Template.Domain.Model.Template>().AddRange(
                               new Template.Domain.Model.Template
                               {
                                   Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                                   Title = "Customer Feedback Survey",
                                   Description = "A template for collecting customer feedback"
                               },
                               new Template.Domain.Model.Template
                               {
                                   Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                                   Title = "Employee Onboarding Checklist",
                                   Description = "A comprehensive onboarding checklist for new employees"
                               }
                           );
                           context.SaveChanges();
                       }

                       if (!context.Set<Question>().Any())
                       {
                           // Seed Questions
                           context.Set<Question>().AddRange(
                               new SingleLineStringQuestion
                               {
                                   Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                                   Title = "What is your name?",
                                   Description = "Please provide your full name"
                               },
                               new SingleLineStringQuestion
                               {
                                   Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                                   Title = "What is your email?",
                                   Description = "Please provide a valid email address"
                               },
                               new PositiveIntegerQuestion
                               {
                                   Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                                   Title = "How satisfied are you?",
                                   Description = "Rate your satisfaction from 1 to 10"
                               }
                           );
                           context.SaveChanges();
                       }
                   }
               })
               .UseAsyncSeeding(async (context, _, cancellationToken) =>
               {
                   // Async seeding - runs after UseSeeding
                   // Only in Development environment
                   var env = builder.Environment;
                   if (env.IsDevelopment())
                   {
                       // Ensure migrations are applied
                       await context.Database.MigrateAsync(cancellationToken);
                   }
               }));
}
// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

// Services
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();


var app = builder.Build();
// Ensure database is created and migrations are applied
if (!builder.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();

    dbContext.Database.Migrate();
}
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("DefaultCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
