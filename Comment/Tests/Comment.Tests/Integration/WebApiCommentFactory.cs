using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Comment.DataAccess.MsSql.Repositories;

namespace Comment.Tests.Integration
{
    public class WebApiCommentFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureTestServices(services =>
            {
                // Add InMemoryDatabase - no need to remove SqlServer as it's not registered in Testing environment
                services.AddDbContext<CommentDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
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
                    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
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
