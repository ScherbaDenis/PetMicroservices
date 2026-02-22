using Microsoft.AspNetCore.Mvc;
using Moq;
using Template.Domain.DTOs;
using Template.Service.Services;
using WebApiTemplate.Controllers;

namespace Template.Tests.Controllers
{
    public class TagControllerTests
    {
        private readonly Mock<ITagService> _mockService;
        private readonly TagController _controller;

        public TagControllerTests()
        {
            _mockService = new Mock<ITagService>();
            _controller = new TagController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithTags()
        {
            // Arrange
            var tags = new List<TagDto>
            {
                new TagDto { Id = 1, Name = "Tag 1" },
                new TagDto { Id = 2, Name = "Tag 2" }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(tags);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTags = Assert.IsAssignableFrom<IEnumerable<TagDto>>(okResult.Value);
            Assert.Equal(2, returnedTags.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithTag_WhenTagExists()
        {
            // Arrange
            var tagId = 1;
            var tag = new TagDto { Id = tagId, Name = "Test tag" };
            _mockService.Setup(s => s.FindAsync(tagId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(tag);

            // Act
            var result = await _controller.GetById(tagId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTag = Assert.IsType<TagDto>(okResult.Value);
            Assert.Equal(tagId, returnedTag.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var tagId = 1;
            _mockService.Setup(s => s.FindAsync(tagId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TagDto?)null);

            // Act
            var result = await _controller.GetById(tagId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenTagIsValid()
        {
            // Arrange
            var tag = new TagDto { Id = 1, Name = "New tag" };
            _mockService.Setup(s => s.CreateAsync(tag, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(tag);

            // Act
            var result = await _controller.Create(tag);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TagController.GetById), createdResult.ActionName);
            Assert.Equal(tag.Id, ((TagDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTagIsNull()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Tag cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenTagIsValid()
        {
            // Arrange
            var tagId = 1;
            var tag = new TagDto { Id = tagId, Name = "Updated tag" };
            _mockService.Setup(s => s.FindAsync(tagId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(tag);
            _mockService.Setup(s => s.UpdateAsync(tag, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(tagId, tag);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenTagIsNull()
        {
            // Arrange
            var tagId = 1;

            // Act
            var result = await _controller.Update(tagId, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Tag cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var tagId = 1;
            var differentId = 2;
            var tag = new TagDto { Id = differentId, Name = "Updated tag" };

            // Act
            var result = await _controller.Update(tagId, tag);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var tagId = 1;
            var tag = new TagDto { Id = tagId, Name = "Updated tag" };
            _mockService.Setup(s => s.FindAsync(tagId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TagDto?)null);

            // Act
            var result = await _controller.Update(tagId, tag);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTagExists()
        {
            // Arrange
            var tagId = 1;
            var tag = new TagDto { Id = tagId, Name = "Test tag" };
            _mockService.Setup(s => s.FindAsync(tagId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(tag);
            _mockService.Setup(s => s.DeleteAsync(tag, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(tagId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTagDoesNotExist()
        {
            // Arrange
            var tagId = 1;
            _mockService.Setup(s => s.FindAsync(tagId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TagDto?)null);

            // Act
            var result = await _controller.Delete(tagId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
