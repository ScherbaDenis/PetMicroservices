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

            // Add mock for _commentRepository.FindAsync
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Domain.Models.Comment { Id = commentDto.Id, Text = commentDto.Text });

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
        public async Task Find_ShouldCallRepositoryFind()
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
            _mockRepo.Verify(r => r.FindAsync(guid, It.IsAny<CancellationToken>()), Times.Once);
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

            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Domain.Models.Comment { Id = commentDto.Id, Text = commentDto.Text });
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

        [Fact]
        public async Task GetByTemplateAsync_ShouldReturnCommentsForTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new Domain.Models.Template { Id = templateId, Title = "Test Template" };
            var comments = new List<Domain.Models.Comment>
            {
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Comment 1", Template = template },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Comment 2", Template = template }
            };
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(comments);

            // Act
            var result = await _service.GetByTemplateAsync(templateId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByTemplateAsync_ShouldReturnEmpty_WhenNoCommentsExist()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var emptyComments = new List<Domain.Models.Comment>();
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(emptyComments);

            // Act
            var result = await _service.GetByTemplateAsync(templateId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task FindAsync_WithPredicate_ShouldReturnMatchingComments()
        {
            // Arrange
            var template = new Domain.Models.Template { Id = Guid.NewGuid(), Title = "Test Template" };
            var comments = new List<Domain.Models.Comment>
            {
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Match", Template = template },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "NoMatch", Template = template }
            };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(comments);

            // Act
            var result = await _service.FindAsync(c => c.Text == "Match");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Match", result.First().Text);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedResults()
        {
            // Arrange
            var template = new Domain.Models.Template { Id = Guid.NewGuid(), Title = "Test Template" };
            var comments = new List<Domain.Models.Comment>
            {
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Comment 1", Template = template },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Comment 2", Template = template }
            };
            _mockRepo.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((comments, 2));

            // Act
            var result = await _service.GetPagedAsync(0, 10);

            // Assert
            Assert.NotNull(result.Items);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
            _mockRepo.Verify(r => r.GetPagedAsync(0, 10, It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HardDeleteAsync_ShouldCallHardDeleteAndSaveChanges()
        {
            // Arrange
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "Test Template" };
            var commentDto = new CommentDto { Id = Guid.NewGuid(), Text = "Test comment", TemplateDto = templateDto };

            // Add mock for _commentRepository.FindAsync
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Domain.Models.Comment { Id = commentDto.Id, Text = commentDto.Text });

            // Act
            await _service.HardDeleteAsync(commentDto);

            // Assert
            _mockRepo.Verify(r => r.HardDeleteAsync(It.IsAny<Domain.Models.Comment>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HardDeleteAsync_ShouldThrow_WhenCommentIsNull()
        {
            // Arrange & Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.HardDeleteAsync((CommentDto?)null!));
        }

        [Fact]
        public async Task GetAllDeletedAsync_ShouldReturnAllDeletedComments()
        {
            // Arrange
            var template = new Domain.Models.Template { Id = Guid.NewGuid(), Title = "Test Template" };
            var deletedComments = new List<Domain.Models.Comment>
            {
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Deleted 1", Template = template, IsDeleted = true },
                new Domain.Models.Comment { Id = Guid.NewGuid(), Text = "Deleted 2", Template = template, IsDeleted = true }
            };
            _mockRepo.Setup(r => r.GetAllDeletedAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(deletedComments);

            // Act
            var result = await _service.GetAllDeletedAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(r => r.GetAllDeletedAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindDeletedAsync_ShouldReturnDeletedComment()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var template = new Domain.Models.Template { Id = Guid.NewGuid(), Title = "Test Template" };
            var deletedComment = new Domain.Models.Comment
            {
                Id = commentId,
                Text = "Deleted comment",
                Template = template,
                IsDeleted = true
            };
            _mockRepo.Setup(r => r.FindDeletedAsync(commentId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(deletedComment);

            // Act
            var result = await _service.FindDeletedAsync(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Deleted comment", result.Text);
            _mockRepo.Verify(r => r.FindDeletedAsync(commentId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindDeletedAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindDeletedAsync(commentId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Domain.Models.Comment?)null);

            // Act
            var result = await _service.FindDeletedAsync(commentId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.FindDeletedAsync(commentId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
