using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Design-time factory for creating TemplateDbContext instances during migrations.
    /// This is needed for EF Core tools to work without a running application.
    /// </summary>
    public class TemplateDbContextFactory : IDesignTimeDbContextFactory<TemplateDbContext>
    {
        public TemplateDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TemplateDbContext>();
            
            // Use a temporary connection string for design-time operations
            // The actual connection string will be provided at runtime
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TemplateDb;Trusted_Connection=True;");
            
            return new TemplateDbContext(optionsBuilder.Options);
        }
    }
}
