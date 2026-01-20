using Microsoft.AspNetCore.Mvc;
using Moq;
using Comment.Domain.DTOs;
using Comment.Domain.Services;
using WebApiComment.Controllers;

namespace Comment.Tests.Controllers
{
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _mockService;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            _mockService = new Mock<ICommentService>();
            _controller = new CommentController(_mockService.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithComments()
        {
            // Arrange
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comments = new List<CommentDto>
            {
                new CommentDto { Id = Guid.NewGuid(), Text = "Comment 1", TemplateDto = templateDto },
                new CommentDto { Id = Guid.NewGuid(), Text = "Comment 2", TemplateDto = templateDto }
            };
            _mockService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(comments);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComments = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(okResult.Value);
            Assert.Equal(2, returnedComments.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnOkWithComment_WhenCommentExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comment = new CommentDto { Id = commentId, Text = "Test comment", TemplateDto = templateDto };
            _mockService.Setup(s => s.FindAsync(commentId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(comment);

            // Act
            var result = await _controller.GetById(commentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComment = Assert.IsType<CommentDto>(okResult.Value);
            Assert.Equal(commentId, returnedComment.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _mockService.Setup(s => s.FindAsync(commentId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((CommentDto?)null);

            // Act
            var result = await _controller.GetById(commentId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetByTemplateId_ShouldReturnOkWithComments()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var templateDto = new TemplateDto { Id = templateId, Title = "Test Template" };
            var comments = new List<CommentDto>
            {
                new CommentDto { Id = Guid.NewGuid(), Text = "Comment 1", TemplateDto = templateDto },
                new CommentDto { Id = Guid.NewGuid(), Text = "Comment 2", TemplateDto = templateDto }
            };
            _mockService.Setup(s => s.GetByTemplateAsync(templateId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(comments);

            // Act
            var result = _controller.GetByTemplateId(templateId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComments = Assert.IsAssignableFrom<IEnumerable<CommentDto>>(okResult.Value);
            Assert.Equal(2, returnedComments.Count());
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenCommentIsValid()
        {
            // Arrange
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comment = new CommentDto { Id = Guid.NewGuid(), Text = "New comment", TemplateDto = templateDto };
            _mockService.Setup(s => s.CreateAsync(comment, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(comment);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(CommentController.GetById), createdResult.ActionName);
            Assert.Equal(comment.Id, ((CommentDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenCommentIsNull()
        {
            // Act
            var result = await _controller.Create(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Comment cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenCommentIsValid()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comment = new CommentDto { Id = commentId, Text = "Updated comment", TemplateDto = templateDto };
            _mockService.Setup(s => s.FindAsync(commentId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(comment);
            _mockService.Setup(s => s.UpdateAsync(comment, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(commentId, comment);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenCommentIsNull()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            // Act
            var result = await _controller.Update(commentId, null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Comment cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comment = new CommentDto { Id = differentId, Text = "Updated comment", TemplateDto = templateDto };

            // Act
            var result = await _controller.Update(commentId, comment);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comment = new CommentDto { Id = commentId, Text = "Updated comment", TemplateDto = templateDto };
            _mockService.Setup(s => s.FindAsync(commentId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((CommentDto?)null);

            // Act
            var result = await _controller.Update(commentId, comment);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenCommentExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var comment = new CommentDto { Id = commentId, Text = "Test comment", TemplateDto = templateDto };
            _mockService.Setup(s => s.FindAsync(commentId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(comment);
            _mockService.Setup(s => s.DeleteAsync(comment, It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _mockService.Setup(s => s.FindAsync(commentId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((CommentDto?)null);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
