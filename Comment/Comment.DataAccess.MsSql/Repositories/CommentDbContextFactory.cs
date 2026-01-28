using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Comment.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Design-time factory for creating CommentDbContext instances during migrations.
    /// This is needed for EF Core tools to work without a running application.
    /// </summary>
    public class CommentDbContextFactory : IDesignTimeDbContextFactory<CommentDbContext>
    {
        public CommentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommentDbContext>();
            
            // Use a temporary connection string for design-time operations
            // The actual connection string will be provided at runtime
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CommentDb;Trusted_Connection=True;");
            
            return new CommentDbContext(optionsBuilder.Options);
        }
    }
}
