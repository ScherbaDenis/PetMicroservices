using Microsoft.Extensions.Logging;
using Moq;
using Template.Domain.Model;
using Template.Domain.DTOs;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class TopicServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITopicRepository> _mockRepo;
        private readonly Mock<ILogger<TopicService>> _mockLogger;
        private readonly TopicService _service;

        public TopicServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITopicRepository>();
            _mockLogger = new Mock<ILogger<TopicService>>();
            _mockUnitOfWork.Setup(uow => uow.TopicRepository).Returns(_mockRepo.Object);

            _service = new TopicService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var topicDto = new TopicDto { Id = 1, Name = "t" };

            await _service.CreateAsync(topicDto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTopicIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync((TopicDto?)null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var topicDto = new TopicDto { Id = 1, Name = "t" };
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Topic { Id = topicDto.Id });

            await _service.DeleteAsync(topicDto);

            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTopicIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync((TopicDto?)null!));
        }

        [Fact]
        public async Task Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Topic> { new Topic() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var result = await _service.FindAsync(t => true);

            Assert.NotNull(result);
            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTopic_WhenFound()
        {
            var expected = new Topic();
            _mockRepo.Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.FindAsync(1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Topic?)null);

            var result = await _service.FindAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTopics()
        {
            var expected = new List<Topic> { new Topic(), new Topic() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var topicDto = new TopicDto { Id = 1, Name = "t" };
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Topic { Id = topicDto.Id });

            await _service.UpdateAsync(topicDto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Topic>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTopicIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync((TopicDto?)null!));
        }
    }
}