using Microsoft.AspNetCore.Mvc;
using Moq;
using Template.Domain.DTOs;
using Template.Domain.Services;
using WebApiTemplate.Controllers;

namespace Template.Tests.Controllers
{
    public class TopicControllerTests
    {
        private readonly Mock<ITopicService> _mockService;
        private readonly TopicController _controller;

        public TopicControllerTests()
        {
            _mockService = new Mock<ITopicService>();
            _controller = new TopicController(_mockService.Object);
        }

        [Fact]
        public async void GetAll_ShouldReturnOkWithTopics()
        {
            // Arrange
            var topics = new List<TopicDto>
            {
                new TopicDto { Id = 1, Name = "Topic 1" },
                new TopicDto { Id = 2, Name = "Topic 2" }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(topics);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<TopicDto>>(okResult.Value);            
            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithTopic_WhenTopicExists()
        {
            // Arrange
            var topicId = 1;
            var topic = new TopicDto { Id = topicId, Name = "Test topic" };
            _mockService.Setup(s => s.FindAsync(topicId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(topic);

            // Act
            var result = await _controller.GetById(topicId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTopic = Assert.IsType<TopicDto>(okResult.Value);
            Assert.Equal(topicId, returnedTopic.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var topicId = 1;
            _mockService.Setup(s => s.FindAsync(topicId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TopicDto?)null);

            // Act
            var result = await _controller.GetById(topicId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenTopicIsValid()
        {
            // Arrange
            var topic = new TopicDto { Id = 1, Name = "New topic" };
            _mockService.Setup(s => s.CreateAsync(topic, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(topic);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TopicController.GetById), createdResult.ActionName);
            Assert.Equal(topic.Id, ((TopicDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTopicIsNull()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Topic cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenTopicIsValid()
        {
            // Arrange
            var topicId = 1;
            var topic = new TopicDto { Id = topicId, Name = "Updated topic" };
            _mockService.Setup(s => s.FindAsync(topicId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(topic);
            _mockService.Setup(s => s.UpdateAsync(topic, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(topicId, topic);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenTopicIsNull()
        {
            // Arrange
            var topicId = 1;

            // Act
            var result = await _controller.Update(topicId, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Topic cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var topicId = 1;
            var differentId = 2;
            var topic = new TopicDto { Id = differentId, Name = "Updated topic" };

            // Act
            var result = await _controller.Update(topicId, topic);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var topicId = 1;
            var topic = new TopicDto { Id = topicId, Name = "Updated topic" };
            _mockService.Setup(s => s.FindAsync(topicId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TopicDto?)null);

            // Act
            var result = await _controller.Update(topicId, topic);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTopicExists()
        {
            // Arrange
            var topicId = 1;
            var topic = new TopicDto { Id = topicId, Name = "Test topic" };
            _mockService.Setup(s => s.FindAsync(topicId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(topic);
            _mockService.Setup(s => s.DeleteAsync(topic, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(topicId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTopicDoesNotExist()
        {
            // Arrange
            var topicId = 1;
            _mockService.Setup(s => s.FindAsync(topicId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TopicDto?)null);

            // Act
            var result = await _controller.Delete(topicId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
