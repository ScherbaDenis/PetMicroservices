using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;
using Template.Tests.Integration;
using Xunit;

namespace Template.Tests.Repositories
{
    public class QuestionRepositoryTests : IClassFixture<WebApiTemplateFactory>
    {
        private readonly WebApiTemplateFactory _factory;

        public QuestionRepositoryTests(WebApiTemplateFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AddAsync_SingleLineStringQuestion_ShouldAddToDatabase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var question = new SingleLineStringQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Test Question",
                Description = "Test Description"
            };

            // Act
            context.Questions.Add(question);
            await context.SaveChangesAsync();

            // Assert
            var retrieved = await context.Questions.FindAsync(question.Id);
            Assert.NotNull(retrieved);
            Assert.IsType<SingleLineStringQuestion>(retrieved);
            Assert.Equal(question.Title, retrieved.Title);
        }

        [Fact]
        public async Task AddAsync_CheckboxQuestionWithOptions_ShouldPersistOptions()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var options = new List<string> { "Option 1", "Option 2", "Option 3" };
            var question = new CheckboxQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Select Options",
                Description = "Choose from the list",
                Options = options
            };

            // Act
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Assert
            var retrieved = await context.Questions
                .OfType<CheckboxQuestion>()
                .FirstOrDefaultAsync(q => q.Id == question.Id);
            
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.Options);
            Assert.Equal(3, retrieved.Options.Count());
            Assert.Contains("Option 1", retrieved.Options);
            Assert.Contains("Option 2", retrieved.Options);
            Assert.Contains("Option 3", retrieved.Options);
        }

        [Fact]
        public async Task AddAsync_CheckboxQuestionWithEmptyOptions_ShouldPersist()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var question = new CheckboxQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Empty Options",
                Options = new List<string>()
            };

            // Act
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Assert
            var retrieved = await context.Questions.FindAsync(question.Id);
            Assert.NotNull(retrieved);
            Assert.IsType<CheckboxQuestion>(retrieved);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllQuestionTypes()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var questions = new List<Question>
            {
                new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Q1" },
                new MultiLineTextQuestion { Id = Guid.NewGuid(), Title = "Q2" },
                new PositiveIntegerQuestion { Id = Guid.NewGuid(), Title = "Q3" },
                new CheckboxQuestion { Id = Guid.NewGuid(), Title = "Q4", Options = new[] { "A", "B" } },
                new BooleanQuestion { Id = Guid.NewGuid(), Title = "Q5" }
            };

            foreach (var q in questions)
            {
                context.Questions.Add(q);
            }
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var retrieved = await context.Questions.ToListAsync();

            // Assert
            Assert.True(retrieved.Count >= 5);
            Assert.Contains(retrieved, q => q is SingleLineStringQuestion);
            Assert.Contains(retrieved, q => q is MultiLineTextQuestion);
            Assert.Contains(retrieved, q => q is PositiveIntegerQuestion);
            Assert.Contains(retrieved, q => q is CheckboxQuestion);
            Assert.Contains(retrieved, q => q is BooleanQuestion);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateQuestion()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var question = new SingleLineStringQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Original Title",
                Description = "Original Description"
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var retrieved = await context.Questions.FindAsync(question.Id);
            Assert.NotNull(retrieved);
            retrieved.Title = "Updated Title";
            retrieved.Description = "Updated Description";
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Assert
            var updated = await context.Questions.FindAsync(question.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated Title", updated.Title);
            Assert.Equal("Updated Description", updated.Description);
        }

        [Fact]
        public async Task UpdateAsync_CheckboxQuestion_ShouldUpdateOptions()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var question = new CheckboxQuestion
            {
                Id = Guid.NewGuid(),
                Title = "Original",
                Options = new[] { "Old1", "Old2" }
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var retrieved = await context.Questions
                .OfType<CheckboxQuestion>()
                .FirstOrDefaultAsync(q => q.Id == question.Id);
            Assert.NotNull(retrieved);
            
            retrieved.Options = new[] { "New1", "New2", "New3" };
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Assert
            var updated = await context.Questions
                .OfType<CheckboxQuestion>()
                .FirstOrDefaultAsync(q => q.Id == question.Id);
            Assert.NotNull(updated);
            Assert.NotNull(updated.Options);
            Assert.Equal(3, updated.Options.Count());
            Assert.Contains("New1", updated.Options);
            Assert.Contains("New2", updated.Options);
            Assert.Contains("New3", updated.Options);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveQuestion()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var question = new BooleanQuestion
            {
                Id = Guid.NewGuid(),
                Title = "To Delete"
            };
            
            context.Questions.Add(question);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Act
            var retrieved = await context.Questions.FindAsync(question.Id);
            Assert.NotNull(retrieved);
            context.Questions.Remove(retrieved);
            await context.SaveChangesAsync();

            // Assert
            var deleted = await context.Questions.FindAsync(question.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task FindAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await context.Questions.FindAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_MultipleQuestionsSameType_ShouldSucceed()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TemplateDbContext>();
            
            var questions = new List<SingleLineStringQuestion>
            {
                new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Q1" },
                new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Q2" },
                new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "Q3" }
            };

            // Act
            foreach (var q in questions)
            {
                context.Questions.Add(q);
            }
            await context.SaveChangesAsync();

            // Assert
            var count = await context.Questions
                .OfType<SingleLineStringQuestion>()
                .CountAsync(q => questions.Select(x => x.Id).Contains(q.Id));
            Assert.Equal(3, count);
        }
    }
}
