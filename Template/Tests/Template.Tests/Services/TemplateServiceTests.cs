using Microsoft.Extensions.Logging;
using Moq;
using Template.Domain.DTOs;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class TemplateServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ITemplateRepository> _mockRepo;
        private readonly Mock<ILogger<TemplateService>> _mockLogger;
        private readonly TemplateService _service;

        public TemplateServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITemplateRepository>();
            _mockLogger = new Mock<ILogger<TemplateService>>();
            _unitOfWork.Setup(uow => uow.TemplateRepository).Returns(_mockRepo.Object);

            _service = new TemplateService(_unitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "t" };

            await _service.CreateAsync(templateDto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Domain.Model.Template>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTemplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync((TemplateDto?)null!));
        }

        [Fact]
        public void Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Domain.Model.Template> { new Domain.Model.Template() };
            _mockRepo.Setup(r => r.Find(It.IsAny<Func<Domain.Model.Template, bool>>())).Returns(expected);

            var result = _service.Find(t => true);

            Assert.NotNull(result);
            _mockRepo.Verify(r => r.Find(It.IsAny<Func<Domain.Model.Template, bool>>()), Times.Once);
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
        public void GetAllAsync_ShouldReturnAllTemplates()
        {
            var expected = new List<Domain.Model.Template> { new Domain.Model.Template(), new Domain.Model.Template() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .Returns(expected);

            var result = _service.GetAllAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var templateDto = new TemplateDto { Id = Guid.NewGuid(), Title = "t" };

            await _service.UpdateAsync(templateDto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Domain.Model.Template>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTemplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync((TemplateDto?)null!));
        }
    }
}