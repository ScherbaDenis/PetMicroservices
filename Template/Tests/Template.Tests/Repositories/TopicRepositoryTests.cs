using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Repositories
{
    public class TopicRepositoryTests
    {
        private readonly TemplateDbContext _context;
        private readonly Mock<ILogger<TopicRepository>> _mockLogger;
        private readonly TopicRepository _repository;

        public TopicRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new TemplateDbContext(options);
            _mockLogger = new Mock<ILogger<TopicRepository>>();
            _repository = new TopicRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTopic()
        {
            var topic = new Topic { Id = 1, Name = "Test Topic" };

            await _repository.AddAsync(topic);
            await _context.SaveChangesAsync();

            var saved = _context.Topics.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Topic", saved!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSoftDeleteTopic()
        {
            var topic = new Topic { Id = 2, Name = "ToDelete" };
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(topic);
            await _context.SaveChangesAsync();

            // Assert - Topic should still exist in database but with IsDeleted = true
            var deletedTopic = await _context.Topics.FindAsync(topic.Id);
            Assert.NotNull(deletedTopic);
            Assert.True(deletedTopic.IsDeleted);
        }

        [Fact]
        public async Task HardDeleteAsync_ShouldPermanentlyRemoveTopic()
        {
            var topic = new Topic { Id = 2, Name = "ToDelete" };
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            await _repository.HardDeleteAsync(topic);
            await _context.SaveChangesAsync();

            // Assert - Topic should be completely removed from database
            var deletedTopic = await _context.Topics.FindAsync(topic.Id);
            Assert.Null(deletedTopic);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTopic_WhenExists()
        {
            var topic = new Topic { Id = 3, Name = "FindMe" };
            _context.Topics.Add(topic);
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
        public async Task GetAllAsync_ShouldReturnAllTopics()
        {
            _context.Topics.AddRange(
                new Topic { Id = 4, Name = "T1" },
                new Topic { Id = 5, Name = "T2" }
            );
            _context.SaveChanges();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyTopic()
        {
            var topic = new Topic { Id = 6, Name = "OldName" };
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            topic.Name = "NewName";
            await _repository.UpdateAsync(topic);
            await _context.SaveChangesAsync();

            var updated = _context.Topics.First(t => t.Id == 6);
            Assert.Equal("NewName", updated.Name);
        }

        [Fact]
        public async Task FindAsync_WithPredicate_ShouldReturnMatchingTopics()
        {
            _context.Topics.AddRange(
                new Topic { Id = 7, Name = "Match" },
                new Topic { Id = 8, Name = "Skip" }
            );
            _context.SaveChanges();

            var result = await _repository.FindAsync(t => t.Name == "Match");

            Assert.Single(result);
            Assert.Equal("Match", result.First().Name);
        }
    }
}