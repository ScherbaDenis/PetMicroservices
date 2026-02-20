using Answer.Api.Consumers;
using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using Answer.Infrastructure.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Answer.Api.IntegrationTests;

public class AnswerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Replace SQL repositories with in-memory ones for testing
            services.RemoveAll<IRepository<User>>();
            services.RemoveAll<IRepository<Question>>();
            services.RemoveAll<IRepository<Answer.Domain.Entities.Template>>();
            services.RemoveAll<IRepository<Answer.Domain.Entities.Answer>>();

            services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();
            services.AddSingleton<IRepository<Question>, InMemoryRepository<Question>>();
            services.AddSingleton<IRepository<Answer.Domain.Entities.Template>, InMemoryRepository<Answer.Domain.Entities.Template>>();
            services.AddSingleton<IRepository<Answer.Domain.Entities.Answer>, InMemoryRepository<Answer.Domain.Entities.Answer>>();

            // Replace RabbitMQ transport with in-memory for tests
            services.AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<TemplateCreatedEventConsumer>();
            });
        });
    }
}
