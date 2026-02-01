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
        public async Task DeleteAsync_ShouldRemoveTemplate()
        {
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title  = "ToDelete" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(template);
            await _context.SaveChangesAsync();

            Assert.Empty(_context.Templates);
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
    }
}