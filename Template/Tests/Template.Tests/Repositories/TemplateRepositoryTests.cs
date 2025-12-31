using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.Tests.Repositories
{
    public class TemplateRepositoryTests
    {
        private readonly TemplateDbContext _context;
        private readonly Mock<ILogger<TemplateRepository>> _mockLogger;
        private readonly TemplateRepository _repository;

        public TemplateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new TemplateDbContext(options);
            _mockLogger = new Mock<ILogger<TemplateRepository>>();
            _repository = new TemplateRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTamplate()
        {
            var tamplate = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Test Tamplate" };

            await _repository.AddAsync(tamplate);
            await _repository.SaveChangesAsync();

            var saved = _context.Tamplates.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Tamplate", saved!.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTamplate()
        {
            var tamplate = new Domain.Model.Template { Id = Guid.NewGuid(), Title  = "ToDelete" };
            _context.Tamplates.Add(tamplate);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(tamplate);
            await _repository.SaveChangesAsync();

            Assert.Empty(_context.Tamplates);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTamplate_WhenExists()
        {
            var guid = Guid.NewGuid();
            var tamplate = new Domain.Model.Template { Id = guid, Title = "FindMe" };
            _context.Tamplates.Add(tamplate);
            await _context.SaveChangesAsync();

            var result = await _repository.FindAsync(guid);

            Assert.NotNull(result);
            Assert.Equal("FindMe", result!.Title);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.FindAsync(Guid.NewGuid());
            Assert.Null(result);
        }

        [Fact]
        public void GetAllAsync_ShouldReturnAllTamplates()
        {
            _context.Tamplates.AddRange(
                new Template { Id = Guid.NewGuid(), Title = "T1" },
                new Template { Id = Guid.NewGuid(), Title = "T2" }
            );
            _context.SaveChanges();

            var result = _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyTamplate()
        {
            var guid = Guid.NewGuid();
            var tamplate = new Domain.Model.Template { Id = guid, Title = "OldName" };
            _context.Tamplates.Add(tamplate);
            await _context.SaveChangesAsync();

            tamplate.Title = "NewName";
            await _repository.UpdateAsync(tamplate);
            await _repository.SaveChangesAsync();

            var updated = _context.Tamplates.First(t => t.Id == guid);
            Assert.Equal("NewName", updated.Title);
        }

        [Fact]
        public void Find_WithPredicate_ShouldReturnMatchingTamplates()
        {
            _context.Tamplates.AddRange(
                new Template { Id = Guid.NewGuid(), Title = "Match" },
                new Template { Id = Guid.NewGuid(), Title = "Skip" }
            );
            _context.SaveChanges();

            var result = _repository.Find(t => t.Title == "Match");

            Assert.Single(result);
            Assert.Equal("Match", result.First().Title);
        }
    }
}