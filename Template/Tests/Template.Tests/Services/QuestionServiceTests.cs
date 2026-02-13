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
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IQuestionRepository> _mockRepo;
        private readonly Mock<ILogger<QuestionService>> _mockLogger;
        private readonly QuestionService _service;

        public QuestionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IQuestionRepository>();
            _mockLogger = new Mock<ILogger<QuestionService>>();
            _mockUnitOfWork.Setup(uow => uow.QuestionRepository).Returns(_mockRepo.Object);

            _service = new QuestionService(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCallAddAndSaveChanges()
        {
            var dto = new SingleLineStringQuestionDto { Id = Guid.NewGuid(), Title = "title", Description = "desc" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenQuestionIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync((QuestionDto?)null!));
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDeleteAndSaveChanges()
        {
            var dto = new SingleLineStringQuestionDto { Id = Guid.NewGuid(), Title = "title", Description = "desc" };

            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new SingleLineStringQuestion { Id = dto.Id });

            await _service.DeleteAsync(dto);

            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenQuestionIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteAsync((QuestionDto?)null!));
        }

        [Fact]
        public async Task Find_ShouldCallRepositoryFind()
        {
            var expected = new List<Question> { new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "title" } };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var result = await _service.FindAsync(q => true);

            Assert.NotNull(result);
            _mockRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnQuestion_WhenFound()
        {
            var expected = new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "title", Description = "desc" };
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
                new SingleLineStringQuestion { Id = Guid.NewGuid(), Title = "One" },
                new BooleanQuestion { Id = Guid.NewGuid(), Title = "Two" }
            };
            _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndSaveChanges()
        {
            var dto = new SingleLineStringQuestionDto { Id = Guid.NewGuid(), Title = "title" };
            _mockRepo.Setup(r => r.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new SingleLineStringQuestion { Id = dto.Id });

            await _service.UpdateAsync(dto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Question>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenQuestionIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync((QuestionDto?)null!));
        }

        [Fact]
        public async Task CreateAsync_ShouldWorkWithSingleLineStringQuestion()
        {
            var dto = new SingleLineStringQuestionDto { Id = Guid.NewGuid(), Title = "Name", Description = "Enter your name" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<SingleLineStringQuestion>(q => q.Title == "Name"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldWorkWithMultiLineTextQuestion()
        {
            var dto = new MultiLineTextQuestionDto { Id = Guid.NewGuid(), Title = "Comments", Description = "Enter your comments" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<MultiLineTextQuestion>(q => q.Title == "Comments"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldWorkWithPositiveIntegerQuestion()
        {
            var dto = new PositiveIntegerQuestionDto { Id = Guid.NewGuid(), Title = "Age", Description = "Enter your age" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<PositiveIntegerQuestion>(q => q.Title == "Age"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldWorkWithCheckboxQuestion()
        {
            var dto = new CheckboxQuestionDto { Id = Guid.NewGuid(), Title = "Options", Description = "Select options" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<CheckboxQuestion>(q => q.Title == "Options"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldWorkWithBooleanQuestion()
        {
            var dto = new BooleanQuestionDto { Id = Guid.NewGuid(), Title = "Agree", Description = "Do you agree?" };

            await _service.CreateAsync(dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<BooleanQuestion>(q => q.Title == "Agree"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
