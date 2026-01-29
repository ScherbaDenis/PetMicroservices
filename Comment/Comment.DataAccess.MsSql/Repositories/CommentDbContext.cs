using Microsoft.EntityFrameworkCore;
using Comment.DataAccess.MsSql.EntityConfigurations;
using Comment.Domain.Models;

namespace Comment.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Represents the Entity Framework database context for the Comment domain.
    /// Manages entity sets and applies entity configurations for the Comment module.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public class CommentDbContext(DbContextOptions<CommentDbContext> options) : DbContext(options)
    {

        public DbSet<Domain.Models.Comment> Comments { get; set; }
        public DbSet<Template> Templates { get; set; }

        /// <summary>
        /// Configures the entity mappings and schema for the context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Comment");

            modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateEntityConfiguration());
        }
    }
}
