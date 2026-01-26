using Microsoft.EntityFrameworkCore;
using Template.DataAccess.MsSql.Configurations;
using Template.DataAccess.MsSql.EntityConfigurations;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    public class TemplateDbContext : DbContext
    {
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Domain.Model.Template> Templates { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("template");

            modelBuilder.ApplyConfiguration(new TagEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new UserEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new TopicEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new TemplateEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new QuestionEntityConfiguration());

            // Seed data for testing
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            var user1 = new User { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "John Doe" };
            var user2 = new User { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Jane Smith" };
            modelBuilder.Entity<User>().HasData(user1, user2);

            // Seed Topics
            modelBuilder.Entity<Topic>().HasData(
                new Topic { Id = 1, Name = "Technology" },
                new Topic { Id = 2, Name = "Science" },
                new Topic { Id = 3, Name = "Education" }
            );

            // Seed Tags
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Programming" },
                new Tag { Id = 2, Name = "Database" },
                new Tag { Id = 3, Name = "Web Development" },
                new Tag { Id = 4, Name = "Machine Learning" }
            );

            // Seed Templates
            var template1 = new Domain.Model.Template
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Title = "Customer Feedback Survey",
                Description = "A template for collecting customer feedback"
            };
            var template2 = new Domain.Model.Template
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Title = "Employee Onboarding Checklist",
                Description = "A comprehensive onboarding checklist for new employees"
            };
            modelBuilder.Entity<Domain.Model.Template>().HasData(template1, template2);

            // Seed Questions
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Title = "What is your name?",
                    Description = "Please provide your full name"
                },
                new Question
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    Title = "What is your email?",
                    Description = "Please provide a valid email address"
                },
                new Question
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    Title = "How satisfied are you?",
                    Description = "Rate your satisfaction from 1 to 10"
                }
            );
        }
    }
}
