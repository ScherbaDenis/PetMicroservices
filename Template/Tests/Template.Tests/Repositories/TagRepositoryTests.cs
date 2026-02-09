using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Repositories
{
    public class TagRepositoryTests
    {
        private readonly TemplateDbContext _context;
        private readonly Mock<ILogger<TagRepository>> _mockLogger;
        private readonly TagRepository _repository;

        public TagRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new TemplateDbContext(options);
            _mockLogger = new Mock<ILogger<TagRepository>>();
            _repository = new TagRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTag()
        {
            var tag = new Tag { Id = 1, Name = "Test Tag" };

            await _repository.AddAsync(tag);
            await _context.SaveChangesAsync();

            var saved = _context.Tags.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Tag", saved!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteTag()
        {
            var tag = new Tag { Id = 2, Name = "ToDelete" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(tag);
            await _context.SaveChangesAsync();

            // Assert - Tag should still exist in database but with IsDeleted = true
            var deletedTag = await _context.Tags.FindAsync(tag.Id);
            Assert.NotNull(deletedTag);
            Assert.True(deletedTag.IsDeleted);
        }

        [Fact]
        public async Task HardDeleteAsync_ShouldPermanentlyRemoveTag()
        {
            var tag = new Tag { Id = 2, Name = "ToDelete" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            await _repository.HardDeleteAsync(tag);
            await _context.SaveChangesAsync();

            // Assert - Tag should be completely removed from database
            var deletedTag = await _context.Tags.FindAsync(tag.Id);
            Assert.Null(deletedTag);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTag_WhenExists()
        {
            var tag = new Tag { Id = 3, Name = "FindMe" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            var result = await _repository.FindAsync(3);

            Assert.NotNull(result);
            Assert.Equal("FindMe", result!.Name);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.FindAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTags()
        {
            _context.Tags.AddRange(
                new Tag { Id = 4, Name = "T1" },
                new Tag { Id = 5, Name = "T2" }
            );
            _context.SaveChanges();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyTag()
        {
            var tag = new Tag { Id = 6, Name = "OldName" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            tag.Name = "NewName";
            await _repository.UpdateAsync(tag);
            await _context.SaveChangesAsync();

            var updated = _context.Tags.First(t => t.Id == 6);
            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public async Task FindAsync_WithPredicate_ShouldReturnMatchingTags()
        {
            _context.Tags.AddRange(
                new Tag { Id = 7, Name = "Match" },
                new Tag { Id = 8, Name = "Skip" }
            );
            _context.SaveChanges();

            var result = await _repository.FindAsync(t => t.Name == "Match");

            Assert.Single(result);
            Assert.Equal("Match", result.First().Name);
        }
    }
}