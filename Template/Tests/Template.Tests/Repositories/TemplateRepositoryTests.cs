using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Repositories
{
    public class TemplateRepositoryTests
    {
        private readonly TemplateDbContext _context;
        private readonly Mock<ILogger<TemplateRepository>> _mockLogger;
        private readonly TemplateRepository _repository;

        public TemplateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new TemplateDbContext(options);
            _mockLogger = new Mock<ILogger<TemplateRepository>>();
            _repository = new TemplateRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTemplate()
        {
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Test Template" };

            await _repository.AddAsync(template);
            await _context.SaveChangesAsync();

            var saved = _context.Templates.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Template", saved!.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteTemplate()
        {
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title  = "ToDelete" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(template);
            await _context.SaveChangesAsync();

            // Assert - Template should still exist in database but with IsDeleted = true
            var deletedTemplate = await _context.Templates.FindAsync(template.Id);
            Assert.NotNull(deletedTemplate);
            Assert.True(deletedTemplate.IsDeleted);
        }

        [Fact]
        public async Task HardDeleteAsync_ShouldPermanentlyRemoveTemplate()
        {
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title  = "ToDelete" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            await _repository.HardDeleteAsync(template);
            await _context.SaveChangesAsync();

            // Assert - Template should be completely removed from database
            var deletedTemplate = await _context.Templates.FindAsync(template.Id);
            Assert.Null(deletedTemplate);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTemplate_WhenExists()
        {
            var guid = Guid.NewGuid();
            var template = new Domain.Model.Template { Id = guid, Title = "FindMe" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            var result = await _repository.FindAsync(guid);

            Assert.NotNull(result);
            Assert.Equal("FindMe", result!.Title);
        }

        [Fact]
        public async Task FindAsync_ShouldIncludeRelatedEntities()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var topicId = 1;
            var tagId = 1;
            var questionId = Guid.NewGuid();
            var templateId = Guid.NewGuid();

            var user = new User { Id = userId, Name = "Test User" };
            var topic = new Topic { Id = topicId, Name = "Test Topic" };
            var tag = new Tag { Id = tagId, Name = "Test Tag" };
            var question = new SingleLineStringQuestion { Id = questionId, Title = "Test Question" };

            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Template with Relations",
                Owner = user,
                Topic = topic,
                Tags = new List<Tag> { tag },
                UsersAccess = new List<User> { user },
                Questions = new List<Question> { question }
            };

            _context.Users.Add(user);
            _context.Topics.Add(topic);
            _context.Tags.Add(tag);
            _context.Questions.Add(question);
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(templateId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result!.Owner);
            Assert.Equal(userId, result.Owner.Id);
            Assert.NotNull(result.Topic);
            Assert.Equal(topicId, result.Topic.Id);
            Assert.NotNull(result.Tags);
            Assert.Single(result.Tags);
            Assert.Equal(tagId, result.Tags.First().Id);
            Assert.NotNull(result.UsersAccess);
            Assert.Single(result.UsersAccess);
            Assert.Equal(userId, result.UsersAccess.First().Id);
            Assert.NotNull(result.Questions);
            Assert.Single(result.Questions);
            Assert.Equal(questionId, result.Questions.First().Id);
        }

        [Fact]
        public async Task FindAsync_WithPredicate_ShouldIncludeRelatedEntities()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var topicId = 1;
            var templateId = Guid.NewGuid();

            var user = new User { Id = userId, Name = "Test User" };
            var topic = new Topic { Id = topicId, Name = "Test Topic" };

            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Template with Relations",
                Owner = user,
                Topic = topic
            };

            _context.Users.Add(user);
            _context.Topics.Add(topic);
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(t => t.Title == "Template with Relations");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var foundTemplate = result.First();
            Assert.NotNull(foundTemplate.Owner);
            Assert.Equal(userId, foundTemplate.Owner.Id);
            Assert.NotNull(foundTemplate.Topic);
            Assert.Equal(topicId, foundTemplate.Topic.Id);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.FindAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTemplates()
        {
            _context.Templates.AddRange(
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "T1" },
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "T2" }
            );
            _context.SaveChanges();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyTemplate()
        {
            var guid = Guid.NewGuid();
            var template = new Domain.Model.Template { Id = guid, Title = "OldName" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            template.Title = "NewName";
            await _repository.UpdateAsync(template);
            await _context.SaveChangesAsync();

            var updated = _context.Templates.First(t => t.Id == guid);
            Assert.Equal("NewName", updated.Title);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnTemplatesOwnedByUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var otherUser = new User { Id = otherUserId, Name = "Other User" };
            _context.Users.Add(user);
            _context.Users.Add(otherUser);
            
            var template1 = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "User Template 1", Owner = user };
            var template2 = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "User Template 2", Owner = user };
            var template3 = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Other Template", Owner = otherUser };
            _context.Templates.AddRange(template1, template2, template3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.Equal(userId, t.Owner!.Id));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnTemplatesUserHasAccessTo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var owner = new User { Id = ownerId, Name = "Owner" };
            _context.Users.Add(user);
            _context.Users.Add(owner);
            
            var template1 = new Domain.Model.Template 
            { 
                Id = Guid.NewGuid(), 
                Title = "Accessible Template", 
                Owner = owner,
                UsersAccess = new List<User> { user }
            };
            var template2 = new Domain.Model.Template 
            { 
                Id = Guid.NewGuid(), 
                Title = "Not Accessible", 
                Owner = owner
            };
            _context.Templates.AddRange(template1, template2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Accessible Template", result.First().Title);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnBothOwnedAndAccessibleTemplates()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var otherUser = new User { Id = otherUserId, Name = "Other User" };
            _context.Users.Add(user);
            _context.Users.Add(otherUser);
            
            var ownedTemplate = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Owned", Owner = user };
            var accessibleTemplate = new Domain.Model.Template 
            { 
                Id = Guid.NewGuid(), 
                Title = "Accessible", 
                Owner = otherUser,
                UsersAccess = new List<User> { user }
            };
            _context.Templates.AddRange(ownedTemplate, accessibleTemplate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AssignTemplateToUserAsync_ShouldAddUserToUsersAccess()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Test Template",
                UsersAccess = new List<User>()
            };
            _context.Users.Add(user);
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AssignTemplateToUserAsync(templateId, userId);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTemplate = await _context.Templates
                .Include(t => t.UsersAccess)
                .FirstAsync(t => t.Id == templateId);
            Assert.NotNull(updatedTemplate.UsersAccess);
            Assert.Single(updatedTemplate.UsersAccess);
            Assert.Equal(userId, updatedTemplate.UsersAccess.First().Id);
        }

        [Fact]
        public async Task AssignTemplateToUserAsync_ShouldNotDuplicateUser_WhenAlreadyAssigned()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Test Template",
                UsersAccess = new List<User> { user }
            };
            _context.Users.Add(user);
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AssignTemplateToUserAsync(templateId, userId);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTemplate = await _context.Templates
                .Include(t => t.UsersAccess)
                .FirstAsync(t => t.Id == templateId);
            Assert.NotNull(updatedTemplate.UsersAccess);
            Assert.Single(updatedTemplate.UsersAccess); // Should still be 1, not 2
        }

        [Fact]
        public async Task AssignTemplateToUserAsync_ShouldThrow_WhenTemplateNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _repository.AssignTemplateToUserAsync(templateId, userId));
        }

        [Fact]
        public async Task AssignTemplateToUserAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Test Template",
                UsersAccess = new List<User>()
            };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _repository.AssignTemplateToUserAsync(templateId, userId));
        }

        [Fact]
        public async Task UnassignTemplateFromUserAsync_ShouldRemoveUserFromUsersAccess()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = "Test User" };
            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Test Template",
                UsersAccess = new List<User> { user }
            };
            _context.Users.Add(user);
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UnassignTemplateFromUserAsync(templateId, userId);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTemplate = await _context.Templates
                .Include(t => t.UsersAccess)
                .FirstAsync(t => t.Id == templateId);
            Assert.NotNull(updatedTemplate.UsersAccess);
            Assert.Empty(updatedTemplate.UsersAccess);
        }

        [Fact]
        public async Task UnassignTemplateFromUserAsync_ShouldDoNothing_WhenUserNotAssigned()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var otherUser = new User { Id = otherUserId, Name = "Other User" };
            var template = new Domain.Model.Template 
            { 
                Id = templateId, 
                Title = "Test Template",
                UsersAccess = new List<User> { otherUser }
            };
            _context.Users.Add(otherUser);
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UnassignTemplateFromUserAsync(templateId, userId);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTemplate = await _context.Templates
                .Include(t => t.UsersAccess)
                .FirstAsync(t => t.Id == templateId);
            Assert.NotNull(updatedTemplate.UsersAccess);
            Assert.Single(updatedTemplate.UsersAccess); // Other user should still be there
        }

        [Fact]
        public async Task UnassignTemplateFromUserAsync_ShouldThrow_WhenTemplateNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _repository.UnassignTemplateFromUserAsync(templateId, userId));
        }

        [Fact]
        public async Task GetAllDeletedAsync_ShouldReturnOnlyDeletedTemplates()
        {
            // Arrange
            var template1 = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Active" };
            var template2 = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Deleted" };
            _context.Templates.Add(template1);
            _context.Templates.Add(template2);
            await _context.SaveChangesAsync();

            // Soft delete template2
            await _repository.DeleteAsync(template2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllDeletedAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Deleted", result.First().Title);
            Assert.True(result.First().IsDeleted);
        }

        [Fact]
        public async Task FindDeletedAsync_ShouldReturnDeletedTemplate()
        {
            // Arrange
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Deleted" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Soft delete the template
            await _repository.DeleteAsync(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindDeletedAsync(template.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Deleted", result.Title);
            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task FindDeletedAsync_ShouldReturnNullForActiveTemplate()
        {
            // Arrange
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Active" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindDeletedAsync(template.Id);

            // Assert
            Assert.Null(result);
        }
    }
}