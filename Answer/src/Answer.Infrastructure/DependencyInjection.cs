using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using Answer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Answer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IRepository<User>, MsSqlRepository<User>>();
        services.AddScoped<IRepository<Question>, MsSqlRepository<Question>>();
        services.AddScoped<IRepository<Template>, MsSqlRepository<Template>>();
        services.AddScoped<IRepository<Domain.Entities.Answer>, MsSqlRepository<Domain.Entities.Answer>>();

        return services;
    }
}
