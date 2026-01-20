using Microsoft.Extensions.Logging;
using Moq;
using Comment.Domain.DTOs;
using Comment.Domain.Repositories;
using Comment.Service.Services;

namespace Comment.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICommentRepository> _mockRepo;
        private readonly Mock<ILogger<CommentService>> _mockLogger;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ICommentRepository>();
            _mockLogger = new Mock<ILogger<CommentService>>();
            _unitOfWork.Setup(uow => uow.CommentRepository).Returns(_mockRepo.Object);

            _service = new CommentService(_unitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            // Arrange
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var commentDto = new CommentDto { Id = Guid.NewGuid(), Text = "Test comment", TemplateDto = templateDto };

            // Act
            await _service.CreateAsync(commentDto);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Models.Comment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenCommentIsNull()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync((CommentDto?)null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            // Arrange
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var commentDto = new CommentDto { Id = Guid.NewGuid(), Text = "Test comment", TemplateDto = templateDto };

            // Act
            await _service.DeleteAsync(commentDto);

            // Assert
            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Domain.Models.Comment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenCommentIsNull()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync((CommentDto?)null!));
        }

        [Fact]
        public void Find_ShouldCallRepositoryFind()
        {
            // Arrange
            var expected = new List<Domain.Models.Comment> { new Domain.Models.Comment() };
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected.FirstOrDefault);

            // Act
            var result = _service.FindAsync(c => true);

            // Assert
            Assert.NotNull(result);
            _mockRepo.Verify(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnComment_WhenFound()
        {
            // Arrange
            var expected = new Domain.Models.Comment();
            var guid = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindAsync(guid, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            // Act
            var result = await _service.FindAsync(guid);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindAsync(guid, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Domain.Models.Comment?)null);

            // Act
            var result = await _service.FindAsync(guid);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllComments()
        {
            // Arrange
            var expected = new List<Domain.Models.Comment> { new Domain.Models.Comment(), new Domain.Models.Comment() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected.AsEnumerable());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Count, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            // Arrange
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var commentDto = new CommentDto { Id = Guid.NewGuid(), Text = "Updated comment", TemplateDto = templateDto };

            // Act
            await _service.UpdateAsync(commentDto);

            // Assert
            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Models.Comment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenCommentIsNull()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync((CommentDto?)null!));
        }
    }
}
