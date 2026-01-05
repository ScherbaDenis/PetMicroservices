using Microsoft.EntityFrameworkCore;
using Comment.DataAccess.MsSql.Repositories;
using Comment.Domain.Models;

namespace Comment.Tests.Repositories
{
    /// <summary>
    /// Test-specific DbContext that overrides the entity configurations
    /// to work properly with InMemory database provider.
    /// </summary>
    public class TestCommentDbContext : CommentDbContext
    {
        public TestCommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Comment entity without the problematic index on navigation property
            modelBuilder.Entity<Domain.Models.Comment>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.Id);
                // Note: Removed HasIndex on Template navigation property as it's not supported by InMemory provider
            });

            // Configure Template entity
            modelBuilder.Entity<Template>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.Id);
                builder.HasIndex(x => x.Title);
            });
        }
    }
}
