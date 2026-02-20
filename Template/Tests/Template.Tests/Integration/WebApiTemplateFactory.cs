using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.DataAccess.MsSql.Repositories;

namespace Template.Tests.Integration
{
    public class WebApiTemplateFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureTestServices(services =>
            {
                // Add InMemoryDatabase - no need to remove SqlServer as it's not registered in Testing environment
                services.AddDbContext<TemplateDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                // Replace RabbitMQ transport with in-memory for tests
                services.AddMassTransitTestHarness();
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up the in-memory database when factory is disposed
                try
                {
                    using var scope = Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
                    db.Database.EnsureDeleted();
                }
                catch
                {
                    // Ignore errors during cleanup
                }
            }
            base.Dispose(disposing);
        }
    }
}
