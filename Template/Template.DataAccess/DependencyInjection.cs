using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;
using Template.Domain.Repository;
using TemplateModel = Template.Domain.Model.Template;

namespace Template.DataAccess.MsSql;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateDataAccess(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (!environment.IsEnvironment("Testing"))
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<TemplateDbContext>(options =>
                options.UseSqlServer(connectionString)
                       .UseSeeding((context, _) =>
                       {
                           if (environment.IsDevelopment())
                           {
                               if (!context.Set<User>().Any())
                               {
                                   var user1 = new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "John Doe" };
                                   var user2 = new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Jane Smith" };
                                   context.Set<User>().AddRange(user1, user2);
                                   context.SaveChanges();
                               }

                               if (!context.Set<Topic>().Any())
                               {
                                   context.Set<Topic>().AddRange(
                                       new Topic { Name = "Technology" },
                                       new Topic { Name = "Science" },
                                       new Topic { Name = "Education" }
                                   );
                                   context.SaveChanges();
                               }

                               if (!context.Set<Tag>().Any())
                               {
                                   context.Set<Tag>().AddRange(
                                       new Tag { Name = "Programming" },
                                       new Tag { Name = "Database" },
                                       new Tag { Name = "Web Development" },
                                       new Tag { Name = "Machine Learning" }
                                   );
                                   context.SaveChanges();
                               }

                               if (!context.Set<TemplateModel>().Any())
                               {
                                   context.Set<TemplateModel>().AddRange(
                                       new TemplateModel
                                       {
                                           Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                                           Title = "Customer Feedback Survey",
                                           Description = "A template for collecting customer feedback"
                                       },
                                       new TemplateModel
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
                           if (environment.IsDevelopment())
                           {
                               await context.Database.MigrateAsync(cancellationToken);
                           }
                       }));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();

        return services;
    }
}
