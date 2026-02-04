using Microsoft.Extensions.Logging;
using Moq;
using Template.Domain.DTOs;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class TemplateServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITemplateRepository> _mockRepo;
        private readonly Mock<ILogger<TemplateService>> _mockLogger;
        private readonly TemplateService _service;

        public TemplateServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITemplateRepository>();
            _mockLogger = new Mock<ILogger<TemplateService>>();
            _mockUnitOfWork.Setup(uow => uow.TemplateRepository).Returns(_mockRepo.Object);

            _service = new TemplateService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "t" };

            await _service.CreateAsync(templateDto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Model.Template>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTemplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync((TemplateDto?)null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "t" };

            await _service.DeleteAsync(templateDto);

            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Domain.Model.Template>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTemplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync((TemplateDto?)null!));
        }

        [Fact]
        public async Task Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Domain.Model.Template> { new Domain.Model.Template() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var result = await _service.FindAsync(t => true);

            Assert.NotNull(result);
            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTemplate_WhenFound()
        {
            var expected = new Domain.Model.Template();
            var guid = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindAsync(guid, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.FindAsync(guid);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotFound()
        {

            var guid = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindAsync(guid, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Domain.Model.Template?)null);

            var result = await _service.FindAsync(guid);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTemplates()
        {
            var expected = new List<Domain.Model.Template> { new Domain.Model.Template(), new Domain.Model.Template() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "t" };

            await _service.UpdateAsync(templateDto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Model.Template>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTemplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync((TemplateDto?)null!));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserTemplates()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expected = new List<Domain.Model.Template> 
            { 
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Template 1" },
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Template 2" }
            };
            _mockRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            // Act
            var result = await _service.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AssignTemplateToUserAsync_ShouldCallRepositoryAndSaveChanges()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            await _service.AssignTemplateToUserAsync(templateId, userId);

            // Assert
            _mockRepo.Verify(r => r.AssignTemplateToUserAsync(templateId, userId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UnassignTemplateFromUserAsync_ShouldCallRepositoryAndSaveChanges()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act
            await _service.UnassignTemplateFromUserAsync(templateId, userId);

            // Assert
            _mockRepo.Verify(r => r.UnassignTemplateFromUserAsync(templateId, userId, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}