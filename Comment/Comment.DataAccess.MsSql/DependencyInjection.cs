using Comment.DataAccess.MsSql.Repositories;
using Comment.Domain.Models;
using Comment.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommentEntity = Comment.Domain.Models.Comment;

namespace Comment.DataAccess.MsSql;

public static class DependencyInjection
{
    public static IServiceCollection AddCommentDataAccess(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        if (!environment.IsEnvironment("Testing"))
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CommentDbContext>(options =>
                options.UseSqlServer(connectionString)
                       .UseSeeding((context, _) =>
                       {
                           if (environment.IsDevelopment())
                           {
                               if (!context.Set<Template>().Any())
                               {
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
                           if (environment.IsDevelopment())
                           {
                               await context.Database.MigrateAsync(cancellationToken);
                           }
                       }));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITemplateRepository, TemplateRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }
}
