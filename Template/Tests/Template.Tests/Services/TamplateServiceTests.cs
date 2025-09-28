using Microsoft.Extensions.Logging;
using Moq;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class TamplateServiceTests
    {
        private readonly Mock<ITamplateRepository> _mockRepo;
        private readonly Mock<ILogger<TamplateService>> _mockLogger;
        private readonly TamplateService _service;

        public TamplateServiceTests()
        {
            _mockRepo = new Mock<ITamplateRepository>();
            _mockLogger = new Mock<ILogger<TamplateService>>();
            _service = new TamplateService(_mockRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var tamplate = new Tamplate();

            await _service.CreateAsync(tamplate);

            _mockRepo.Verify(r => r.AddAsync(tamplate, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTamplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var tamplate = new Tamplate();

            await _service.DeleteAsync(tamplate);

            _mockRepo.Verify(r => r.DeleteAsync(tamplate, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTamplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync(null!));
        }

        [Fact]
        public void Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Tamplate> { new Tamplate() };
            _mockRepo.Setup(r => r.Find(It.IsAny<Func<Tamplate, bool>>())).Returns(expected);

            var result = _service.Find(t => true);

            Assert.Equal(expected, result);
            _mockRepo.Verify(r => r.Find(It.IsAny<Func<Tamplate, bool>>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTamplate_WhenFound()
        {
            var expected = new Tamplate();
            var guid = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindAsync(guid, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.FindAsync(guid);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotFound()
        {

            var guid = Guid.NewGuid();
            _mockRepo.Setup(r => r.FindAsync(guid, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Tamplate?)null);

            var result = await _service.FindAsync(guid);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllAsync_ShouldReturnAllTamplates()
        {
            var expected = new List<Tamplate> { new Tamplate(), new Tamplate() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .Returns(expected);

            var result = _service.GetAllAsync();

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var tamplate = new Tamplate();

            await _service.UpdateAsync(tamplate);

            _mockRepo.Verify(r => r.UpdateAsync(tamplate, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTamplateIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
        }
    }
}