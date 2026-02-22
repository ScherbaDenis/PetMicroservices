using Microsoft.AspNetCore.Mvc;
using Moq;
using Comment.Domain.DTOs;
using Comment.Service.Services;
using WebApiComment.Controllers;

namespace Comment.Tests.Controllers
{
    public class TemplateControllerTests
    {
        private readonly Mock<ITemplateService> _mockService;
        private readonly TemplateController _controller;

        public TemplateControllerTests()
        {
            _mockService = new Mock<ITemplateService>();
            _controller = new TemplateController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithTemplates()
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
            var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<TemplateDto>>(okResult.Value);
            Assert.Equal(2, returnedTemplates.Count());
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
        public async Task GetAll_ShouldReturnOkWithEmptyList_WhenNoTemplatesExist()
        {
            // Arrange
            var emptyTemplates = new List<TemplateDto>();
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(emptyTemplates);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTemplates = Assert.IsAssignableFrom<IEnumerable<TemplateDto>>(okResult.Value);
            Assert.Empty(returnedTemplates);
        }
    }
}
