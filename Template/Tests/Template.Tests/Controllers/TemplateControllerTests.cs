using Microsoft.AspNetCore.Mvc;
using Moq;
using Template.Domain.DTOs;
using Template.Domain.Services;
using WebApiTemplate.Controllers;

namespace Template.Tests.Controllers
{
    public class TemplateControllerTests
    {
        private readonly Mock<ITemplateService> _mockService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly TemplateController _controller;

        public TemplateControllerTests()
        {
            _mockService = new Mock<ITemplateService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new TemplateController(_mockService.Object, _mockUserService.Object);
        }

        [Fact]
        public async void GetAll_ShouldReturnOkWithTemplates()
        {
            // Arrange
            var templates = new List<TemplateDto>
            {
                new TemplateDto { Id = Guid.NewGuid(), Title = "Template 1" },
                new TemplateDto { Id = Guid.NewGuid(), Title = "Template 2" }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<TemplateDto>>(okResult.Value);
            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithTemplate_WhenTemplateExists()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new TemplateDto { Id = templateId, Title = "Test template" };
            _mockService.Setup(s => s.FindAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(template);

            // Act
            var result = await _controller.GetById(templateId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTemplate = Assert.IsType<TemplateDto>(okResult.Value);
            Assert.Equal(templateId, returnedTemplate.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            _mockService.Setup(s => s.FindAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TemplateDto?)null);

            // Act
            var result = await _controller.GetById(templateId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenTemplateIsValid()
        {
            // Arrange
            var template = new TemplateDto { Id = Guid.NewGuid(), Title = "New template" };
            _mockService.Setup(s => s.CreateAsync(template, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(template);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TemplateController.GetById), createdResult.ActionName);
            Assert.Equal(template.Id, ((TemplateDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenTemplateIsNull()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Template cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenTemplateIsValid()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new TemplateDto { Id = templateId, Title = "Updated template" };
            _mockService.Setup(s => s.FindAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(template);
            _mockService.Setup(s => s.UpdateAsync(template, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(templateId, template);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenTemplateIsNull()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            // Act
            var result = await _controller.Update(templateId, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Template cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var template = new TemplateDto { Id = differentId, Title = "Updated template" };

            // Act
            var result = await _controller.Update(templateId, template);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new TemplateDto { Id = templateId, Title = "Updated template" };
            _mockService.Setup(s => s.FindAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TemplateDto?)null);

            // Act
            var result = await _controller.Update(templateId, template);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenTemplateExists()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new TemplateDto { Id = templateId, Title = "Test template" };
            _mockService.Setup(s => s.FindAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(template);
            _mockService.Setup(s => s.DeleteAsync(template, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(templateId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenTemplateDoesNotExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            _mockService.Setup(s => s.FindAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((TemplateDto?)null);

            // Act
            var result = await _controller.Delete(templateId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnOkWithTemplates_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserDto { Id = userId, Name = "Test user" };
            var templates = new List<TemplateDto>
            {
                new TemplateDto { Id = Guid.NewGuid(), Title = "Template 1", Description = "Description 1" },
                new TemplateDto { Id = Guid.NewGuid(), Title = "Template 2", Description = "Description 2" }
            };
            _mockUserService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
            _mockService.Setup(s => s.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<TemplateDto>>(okResult.Value);
            Assert.Equal(2, returnedTemplates.Count());
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserService.Setup(s => s.FindAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetByUserId(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AssignTemplateToUser_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockService.Setup(s => s.AssignTemplateToUserAsync(templateId, userId, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignTemplateToUser(templateId, userId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockService.Verify(s => s.AssignTemplateToUserAsync(templateId, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignTemplateToUser_ShouldReturnNotFound_WhenTemplateOrUserNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockService.Setup(s => s.AssignTemplateToUserAsync(templateId, userId, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Template or user not found"));

            // Act
            var result = await _controller.AssignTemplateToUser(templateId, userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Template or user not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UnassignTemplateFromUser_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockService.Setup(s => s.UnassignTemplateFromUserAsync(templateId, userId, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UnassignTemplateFromUser(templateId, userId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockService.Verify(s => s.UnassignTemplateFromUserAsync(templateId, userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnassignTemplateFromUser_ShouldReturnNotFound_WhenTemplateNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockService.Setup(s => s.UnassignTemplateFromUserAsync(templateId, userId, It.IsAny<CancellationToken>()))
                       .ThrowsAsync(new InvalidOperationException("Template not found"));

            // Act
            var result = await _controller.UnassignTemplateFromUser(templateId, userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Template not found", notFoundResult.Value);
        }
    }
}
