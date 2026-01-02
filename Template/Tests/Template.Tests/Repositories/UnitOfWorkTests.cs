using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Repository;
using Xunit;

namespace Template.Tests.Repositories
{
    public class UnitOfWorkTests
    {
    private TemplateDbContext? _mockContext;
        private readonly Mock<ILogger<UnitOfWork>> _mockLogger = new();
        private readonly Mock<ILoggerFactory> _mockLoggerFactory = new();

        private UnitOfWork CreateUnitOfWork()
        {
            // Setup logger factory to return mock loggers for repositories
            _mockLoggerFactory
                .Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(new Mock<ILogger>().Object);
            _mockContext = CreateTestDbContext();

            return new UnitOfWork(_mockContext, _mockLogger.Object, _mockLoggerFactory.Object);
        }

        private TemplateDbContext CreateTestDbContext()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            return new TemplateDbContext(options);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenContextIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(null!, _mockLogger.Object, _mockLoggerFactory.Object));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(CreateTestDbContext(), null!, _mockLoggerFactory.Object));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenLoggerFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new UnitOfWork(CreateTestDbContext(), _mockLogger.Object, null!));
        }

        [Fact]
        public void TemplateRepository_ShouldReturn_Instance()
        {
            var uow = CreateUnitOfWork();
            var repo = uow.TemplateRepository;

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<ITemplateRepository>(repo);
            Assert.Same(repo, uow.TemplateRepository); // cached
        }

        [Fact]
        public void TopicRepository_ShouldReturn_Instance()
        {
            var uow = CreateUnitOfWork();
            var repo = uow.TopicRepository;

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<ITopicRepository>(repo);
            Assert.Same(repo, uow.TopicRepository);
        }

        [Fact]
        public void UserRepository_ShouldReturn_Instance()
        {
            var uow = CreateUnitOfWork();
            var repo = uow.UserRepository;

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<IUserRepository>(repo);
            Assert.Same(repo, uow.UserRepository);
        }

        [Fact]
        public void TagRepository_ShouldReturn_Instance()
        {
            var uow = CreateUnitOfWork();
            var repo = uow.TagRepository;

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<ITagRepository>(repo);
            Assert.Same(repo, uow.TagRepository);
        }

        //[Fact]
        //public async Task SaveChangesAsync_ShouldCall_DbContext()
        //{
        //    var uow = CreateUnitOfWork();

        //    _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
        //                .ReturnsAsync(5);

        //    var result = await uow.SaveChangesAsync();

        //    Assert.Equal(5, result);
        //    _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        //}

        //[Fact]
        //public void Dispose_ShouldDispose_DbContext()
        //{
        //    var uow = CreateUnitOfWork();

        //    uow.Dispose();

        //    _mockContext.Verify(c => c.Dispose(), Times.Once);
        //}
    }
}
