using Moq;
using Template.Service.Services;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace MyApp.Tests
{
    public class TopicServiceTests
    {
        private readonly Mock<ITopicRepository> _mockRepo;
        private readonly TopicService _service;

        public TopicServiceTests()
        {
            _mockRepo = new Mock<ITopicRepository>();
            _service = new TopicService(_mockRepo.Object);
        }

        [Fact]
        public void CreateAsync_ShouldCallRepositoryAdd()
        {
            // Arrange
            var topic = new Topic();

            // Act
            _service.CreateAsync(topic, CancellationToken.None);

            // Assert
            _mockRepo.Verify(r => r.CreateAsync(topic, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void DeleteAsync_ShouldCallRepositoryDelete()
        {
            var topic = new Topic();
            _service.DeleteAsync(topic, CancellationToken.None);

            _mockRepo.Verify(r => r.Delete(topic.Id), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTopicFromRepository()
        {
            var expected = new Topic();
            _mockRepo.Setup(r => r.FindAsync(expected.Id, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.FindAsync(CancellationToken.None);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetAllAsync_ShouldReturnAllTopics()
        {
            var expected = new List<Topic> { new Topic(), new Topic() };
            _mockRepo.Setup(r => r.GetAll())
                     .Returns(expected);

            var result = _service.GetAllAsync(CancellationToken.None);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void UpdateAsync_ShouldCallRepositoryUpdate()
        {
            var topic = new Topic();
            _service.UpdateAsync(topic, CancellationToken.None);

            _mockRepo.Verify(r => r.Update(topic), Times.Once);
        }
    }
}
