using Microsoft.Extensions.Logging;
using Moq;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class TopicServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ITopicRepository> _mockRepo;
        private readonly Mock<ILogger<TopicService>> _mockLogger;
        private readonly TopicService _service;

        public TopicServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITopicRepository>();
            _mockLogger = new Mock<ILogger<TopicService>>();
            _unitOfWork.Setup(uow => uow.TopicRepository).Returns(_mockRepo.Object);

            _service = new TopicService(_unitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var topic = new Topic();

            await _service.CreateAsync(topic);

            _mockRepo.Verify(r => r.AddAsync(topic, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTopicIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var topic = new Topic();

            await _service.DeleteAsync(topic);

            _mockRepo.Verify(r => r.DeleteAsync(topic, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTopicIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync(null!));
        }

        [Fact]
        public void Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Topic> { new Topic() };
            _mockRepo.Setup(r => r.Find(It.IsAny<Func<Topic, bool>>())).Returns(expected);

            var result = _service.Find(t => true);

            Assert.Equal(expected, result);
            _mockRepo.Verify(r => r.Find(It.IsAny<Func<Topic, bool>>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTopic_WhenFound()
        {
            var expected = new Topic();
            _mockRepo.Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.FindAsync(1);

            Assert.Equal(expected, result);
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
        public void GetAllAsync_ShouldReturnAllTopics()
        {
            var expected = new List<Topic> { new Topic(), new Topic() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .Returns(expected);

            var result = _service.GetAllAsync();

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var topic = new Topic();

            await _service.UpdateAsync(topic);

            _mockRepo.Verify(r => r.UpdateAsync(topic, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTopicIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
        }
    }
}