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
            // Configure Comment entity without applying the main configuration
            // to work properly with InMemory database provider
            modelBuilder.Entity<Domain.Models.Comment>(builder =>
            {
                builder.HasKey(x => x.Id);
                
                builder.Property(x => x.Text)
                    .IsRequired()
                    .HasMaxLength(2000);
                
                // Configure shadow property for TemplateId
                builder.Property<Guid?>("TemplateId");
                builder.HasIndex("TemplateId");
                
                // Note: Foreign key relationship is simplified for InMemory provider
                builder.HasOne(x => x.Template)
                    .WithMany()
                    .HasForeignKey("TemplateId");
            });

            // Configure Template entity
            modelBuilder.Entity<Template>(builder =>
            {
                builder.HasKey(x => x.Id);
                
                builder.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                    
                builder.HasIndex(x => x.Title);
            });
        }
    }
}
