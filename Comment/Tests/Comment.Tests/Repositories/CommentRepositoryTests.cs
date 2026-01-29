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
        public async Task DeleteAsync_ShouldRemoveComment()
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

            // Assert
            Assert.Empty(_context.Comments);
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
        public void GetAllAsync_ShouldReturnAllComments()
        {
            // Arrange
            var template = new Template { Id = Guid.NewGuid(), Title = "Test Template" };
            _context.Templates.Add(template);
            _context.Comments.AddRange(
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "C1", Template = template },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "C2", Template = template }
            );
            _context.SaveChanges();

            // Act
            IEnumerable<Domain.Models.Comment> result = _repository.GetAllAsync().Result;

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
    }
}
