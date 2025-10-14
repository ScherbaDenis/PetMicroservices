using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class TagServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ITagRepository> _mockRepo;
        private readonly Mock<ILogger<TagService>> _mockLogger;
        private readonly TagService _service;

        public TagServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITagRepository>();
            _mockLogger = new Mock<ILogger<TagService>>();
            _unitOfWork.Setup(uow => uow.TagRepository).Returns(_mockRepo.Object);

            _service = new TagService(_unitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var tag = new Tag();

            await _service.CreateAsync(tag);

            _mockRepo.Verify(r => r.AddAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenTagIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var tag = new Tag();

            await _service.DeleteAsync(tag);

            _mockRepo.Verify(r => r.DeleteAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenTagIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync(null!));
        }

        [Fact]
        public void Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Tag> { new Tag() };
            _mockRepo.Setup(r => r.Find(It.IsAny<Func<Tag, bool>>())).Returns(expected);

            var result = _service.Find(t => true);

            Assert.Equal(expected, result);
            _mockRepo.Verify(r => r.Find(It.IsAny<Func<Tag, bool>>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTag_WhenFound()
        {
            var expected = new Tag();
            _mockRepo.Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var result = await _service.FindAsync(1);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.FindAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Tag?)null);

            var result = await _service.FindAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllAsync_ShouldReturnAllTags()
        {
            var expected = new List<Tag> { new Tag(), new Tag() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .Returns(expected);

            var result = _service.GetAllAsync();

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var tag = new Tag();

            await _service.UpdateAsync(tag);

            _mockRepo.Verify(r => r.UpdateAsync(tag, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenTagIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
        }
    }
}