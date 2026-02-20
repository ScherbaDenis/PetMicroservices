using Answer.Api.Consumers;
using Answer.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Answer.Api.IntegrationTests;

public class AnswerApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // Provide EF Core in-memory database for testing
            services.AddDbContext<AnswerDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            // Replace RabbitMQ transport with in-memory for tests
            services.AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<TemplateCreatedEventConsumer>();
            });
        });
    }
}
