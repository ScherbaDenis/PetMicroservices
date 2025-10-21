using Microsoft.Extensions.Logging;
using Moq;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _unitOfWork.Setup(uow => uow.UserRepository).Returns(_mockRepo.Object);

            _service = new UserService(_unitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var user = new User();

            await _service.CreateAsync(user);

            _mockRepo.Verify(r => r.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenUserIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var user = new User();

            await _service.DeleteAsync(user);

            _mockRepo.Verify(r => r.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenUserIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync(null!));
        }

        [Fact]
        public void Find_ShouldCallRepositoryFind()
        {
            var expected = new List<User> { new User() };
            _mockRepo.Setup(r => r.Find(It.IsAny<Func<User, bool>>())).Returns(expected);

            var result = _service.Find(t => true);

            Assert.Equal(expected, result);
            _mockRepo.Verify(r => r.Find(It.IsAny<Func<User, bool>>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnUser_WhenFound()
        {
            var expected = new User();
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
                     .ReturnsAsync((User?)null);

            var result = await _service.FindAsync(guid);

            Assert.Null(result);
        }

        [Fact]
        public void GetAllAsync_ShouldReturnAllUsers()
        {
            var expected = new List<User> { new User(), new User() };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .Returns(expected);

            var result = _service.GetAllAsync();

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var user = new User();

            await _service.UpdateAsync(user);

            _mockRepo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenUserIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null!));
        }
    }
}