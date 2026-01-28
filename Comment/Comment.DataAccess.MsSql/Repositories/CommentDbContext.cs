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
            // Seed Templates
            var template1 = new Template
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Title = "Customer Feedback Template"
            };
            var template2 = new Template
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Title = "Product Review Template"
            };
            var template3 = new Template
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Title = "Support Ticket Template"
            };
            
            modelBuilder.Entity<Template>().HasData(template1, template2, template3);

            // Seed Comments - initial sample data for demonstration
            modelBuilder.Entity<Domain.Models.Comment>().HasData(
                new
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Text = "Great product! Highly recommended.",
                    TemplateId = (Guid?)template1.Id
                },
                new
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Text = "The service was excellent and very helpful.",
                    TemplateId = (Guid?)template1.Id
                },
                new
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Text = "Good quality, but a bit expensive.",
                    TemplateId = (Guid?)template2.Id
                },
                new
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Text = "Fast delivery and good packaging.",
                    TemplateId = (Guid?)template2.Id
                },
                new
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Text = "Issue resolved quickly by support team.",
                    TemplateId = (Guid?)template3.Id
                }
            );
        }
    }
}
