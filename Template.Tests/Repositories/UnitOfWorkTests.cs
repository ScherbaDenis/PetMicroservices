using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Repository;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System;

namespace Template.Tests.Repositories
{
    public class UnitOfWorkTests
    {
        private TamplateDbContext? _mockContext;
        private readonly Mock<ILogger<UnitOfWork>> _mockLogger = new();
        private readonly Mock<ILoggerFactory> _mockLoggerFactory = new();

        private UnitOfWork CreateUnitOfWork()
        {
            _mockLoggerFactory
                .Setup(f => f.CreateLogger(It.IsAny<string>()))
                .Returns(new Mock<ILogger>().Object);
            _mockContext = CreateTestDbContext();
            return new UnitOfWork(_mockContext, _mockLogger.Object, _mockLoggerFactory.Object);
        }

        private TamplateDbContext CreateTestDbContext()
        {
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<TamplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new TamplateDbContext(options);
        }

        [Fact]
        public void QuestionRepository_ShouldReturn_Instance()
        {
            var uow = CreateUnitOfWork();
            var repo = uow.QuestionRepository;

            Assert.NotNull(repo);
            Assert.IsAssignableFrom<IQuestionRepository>(repo);
            Assert.Same(repo, uow.QuestionRepository); // cached
        }
    }
}
