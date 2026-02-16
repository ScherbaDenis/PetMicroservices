using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Answer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Answer.Api.IntegrationTests;

public class AnswerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override with test configuration
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["UseInMemoryDatabase"] = "true"
            }!);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AnswerDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        });
    }
}
