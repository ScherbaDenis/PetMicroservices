using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Service.Services;

namespace Template.Tests.Services
{
    public class QuestionServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IQuestionRepository> _mockRepo;
        private readonly Mock<ILogger<QuestionService>> _mockLogger;
        private readonly QuestionService _service;

        public QuestionServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IQuestionRepository>();
            _mockLogger = new Mock<ILogger<QuestionService>>();
            _unitOfWork.Setup(uow => uow.QuestionRepository).Returns(_mockRepo.Object);

            _service = new QuestionService(_unitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var dto = new QuestionDto { Id = Guid.NewGuid(), Title = "title", Description = "desc" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var dto = new QuestionDto { Id = Guid.NewGuid(), Title = "title", Description = "desc" };

            await _service.DeleteAsync(dto);

            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Question> { new Question { Id = Guid.NewGuid(), Title = "title" } };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var result = await _service.FindAsync(q => true);

            Assert.NotNull(result);
            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnQuestion_WhenFound()
        {
            var expected = new Question { Id = Guid.NewGuid(), Title = "title", Description = "desc" };
            _mockRepo.Setup(r => r.FindAsync(expected.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _service.FindAsync(expected.Id);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Question?)null);

            var result = await _service.FindAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllQuestions()
        {
            var expected = new List<Question>
            {
                new Question { Id = Guid.NewGuid(), Title = "One" },
                new Question { Id = Guid.NewGuid(), Title = "Two" }
            };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var dto = new QuestionDto { Id = Guid.NewGuid(), Title = "title" };

            await _service.UpdateAsync(dto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
