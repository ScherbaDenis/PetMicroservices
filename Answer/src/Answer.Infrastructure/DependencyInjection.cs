using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using Answer.Infrastructure.Data;
using Answer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Answer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var useInMemory = configuration.GetValue<bool>("UseInMemoryDatabase", true);
        
        if (useInMemory)
        {
            // Use In-Memory repositories
            services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();
            services.AddSingleton<IRepository<Question>, InMemoryRepository<Question>>();
            services.AddSingleton<IRepository<Template>, InMemoryRepository<Template>>();
            services.AddSingleton<IRepository<Domain.Entities.Answer>, InMemoryRepository<Domain.Entities.Answer>>();
        }
        else
        {
            // Use SQL Server with Entity Framework Core
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            services.AddDbContext<AnswerDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IRepository<User>, MsSqlRepository<User>>();
            services.AddScoped<IRepository<Question>, MsSqlRepository<Question>>();
            services.AddScoped<IRepository<Template>, MsSqlRepository<Template>>();
            services.AddScoped<IRepository<Domain.Entities.Answer>, MsSqlRepository<Domain.Entities.Answer>>();
        }
        
        return services;
    }
}
