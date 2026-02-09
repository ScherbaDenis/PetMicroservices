using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Comment.DataAccess.MsSql.Repositories;
using Comment.Domain.Models;

namespace Comment.Tests.Repositories
{
    public class CommentRepositoryTests
    {
        private readonly TestCommentDbContext _context;
        private readonly Mock<ILogger<CommentRepository>> _mockLogger;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CommentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new TestCommentDbContext(options);
            _mockLogger = new Mock<ILogger<CommentRepository>>();
            _repository = new CommentRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddComment()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            var comment = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Test Comment", Template = template };

            // Act
            await _repository.AddAsync(comment);
            await _context.SaveChangesAsync();

            // Assert
            var saved = _context.Comments.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Comment", saved!.Text);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteComment()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "ToDelete", Template = template };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(comment);
            await _context.SaveChangesAsync();

            // Assert - Comment should still exist in database but with IsDeleted = true
            var deletedComment = await _context.Comments.FindAsync(comment.Id);
            Assert.NotNull(deletedComment);
            Assert.True(deletedComment.IsDeleted);
        }

        [Fact]
        public async Task HardDeleteAsync_ShouldPermanentlyRemoveComment()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "ToDelete", Template = template };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.HardDeleteAsync(comment);
            await _context.SaveChangesAsync();

            // Assert - Comment should be completely removed from database
            var deletedComment = await _context.Comments.FindAsync(comment.Id);
            Assert.Null(deletedComment);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnComment_WhenExists()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = guid, Text = "FindMe", Template = template };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(guid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("FindMe", result!.Text);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange & Act
            var result = await _repository.FindAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllComments()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            _context.Comments.AddRange(
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "C1", Template = template },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "C2", Template = template }
            );
            await _context.SaveChangesAsync();

            // Act
            IEnumerable<Domain.Models.Comment> result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyComment()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = guid, Text = "OldText", Template = template };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Act
            comment.Text = "NewText";
            await _repository.UpdateAsync(comment);
            await _context.SaveChangesAsync();

            // Assert
            var updated = _context.Comments.First(c => c.Id == guid);
            Assert.Equal("NewText", updated.Text);
        }

        [Fact]
        public async void Find_WithPredicate_ShouldReturnMatchingComments()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            _context.Comments.AddRange(
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Match", Template = template },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Skip", Template = template }
            );
            _context.SaveChanges();

            // Act
            var result = await _repository.FindAsync(c => c.Text == "Match");

            // Assert
            Assert.Single(result);
            Assert.Equal("Match", result.First().Text);
        }

        [Fact]
        public async Task AddAsync_ShouldSetDateCreated()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "New comment", Template = template };

            // Act
            var beforeAdd = DateTime.UtcNow;
            await _repository.AddAsync(comment);
            await _context.SaveChangesAsync();
            var afterAdd = DateTime.UtcNow;

            // Assert
            var savedComment = await _context.Comments.FindAsync(comment.Id);
            Assert.NotNull(savedComment);
            Assert.True(savedComment.DateCreated >= beforeAdd && savedComment.DateCreated <= afterAdd);
            Assert.True(savedComment.DateUpdated >= beforeAdd && savedComment.DateUpdated <= afterAdd);
            Assert.False(savedComment.IsDeleted);
        }

        [Fact]
        public async Task UpdateAsync_ShouldSetDateUpdated()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Original", Template = template };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            
            var originalDateUpdated = comment.DateUpdated;
            await Task.Delay(10); // Small delay to ensure different timestamp

            // Act
            comment.Text = "Updated";
            var beforeUpdate = DateTime.UtcNow;
            await _repository.UpdateAsync(comment);
            await _context.SaveChangesAsync();

            // Assert
            var updatedComment = await _context.Comments.FindAsync(comment.Id);
            Assert.NotNull(updatedComment);
            Assert.True(updatedComment.DateUpdated > originalDateUpdated);
            Assert.True(updatedComment.DateUpdated >= beforeUpdate);
        }

        [Fact]
        public async Task GetAllAsync_ShouldExcludeSoftDeletedComments()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment1 = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Active", Template = template };
            var comment2 = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Deleted", Template = template };
            _context.Comments.Add(comment1);
            _context.Comments.Add(comment2);
            await _context.SaveChangesAsync();

            // Soft delete comment2
            await _repository.DeleteAsync(comment2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Active", result.First().Text);
        }

        [Fact]
        public async Task FindAsync_ShouldNotReturnSoftDeletedComment()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            var comment = new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "ToDelete", Template = template };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Soft delete the comment
            await _repository.DeleteAsync(comment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindAsync(comment.Id);

            // Assert
            Assert.Null(result);
        }
    }
}
