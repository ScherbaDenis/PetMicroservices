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

            modelBuilder.ApplyConfiguration(new CommentEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new TemplateEntityConfigurtion());

            // Seed data for testing
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Templates - using unique GUIDs for Comment microservice
            var template1 = new Template
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Title = "Customer Feedback Template"
            };
            var template2 = new Template
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Title = "Product Review Template"
            };
            var template3 = new Template
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Title = "Support Ticket Template"
            };
            
            modelBuilder.Entity<Template>().HasData(template1, template2, template3);

            // Seed Comments - initial sample data for demonstration
            modelBuilder.Entity<Domain.Models.Comment>().HasData(
                new
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                    Text = "Great product! Highly recommended.",
                    TemplateId = (Guid?)template1.Id
                },
                new
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    Text = "The service was excellent and very helpful.",
                    TemplateId = (Guid?)template1.Id
                },
                new
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                    Text = "Good quality, but a bit expensive.",
                    TemplateId = (Guid?)template2.Id
                },
                new
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                    Text = "Fast delivery and good packaging.",
                    TemplateId = (Guid?)template2.Id
                },
                new
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                    Text = "Issue resolved quickly by support team.",
                    TemplateId = (Guid?)template3.Id
                }
            );
        }
    }
}
