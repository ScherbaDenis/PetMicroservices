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
        public async Task AddAsync_ShouldAddTemplate()
        {
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Test Template" };

            await _repository.AddAsync(template);
            await _repository.SaveChangesAsync();

            var saved = _context.Templates.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Test Template", saved!.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveTemplate()
        {
            var template = new Domain.Model.Template { Id = Guid.NewGuid(), Title  = "ToDelete" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(template);
            await _repository.SaveChangesAsync();

            Assert.Empty(_context.Templates);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnTemplate_WhenExists()
        {
            var guid = Guid.NewGuid();
            var template = new Domain.Model.Template { Id = guid, Title = "FindMe" };
            _context.Templates.Add(template);
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
        public void GetAllAsync_ShouldReturnAllTemplates()
        {
            _context.Templates.AddRange(
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "T1" },
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "T2" }
            );
            _context.SaveChanges();

            var result = _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyTemplate()
        {
            var guid = Guid.NewGuid();
            var template = new Domain.Model.Template { Id = guid, Title = "OldName" };
            _context.Templates.Add(template);
            await _context.SaveChangesAsync();

            template.Title = "NewName";
            await _repository.UpdateAsync(template);
            await _repository.SaveChangesAsync();

            var updated = _context.Templates.First(t => t.Id == guid);
            Assert.Equal("NewName", updated.Title);
        }

        [Fact]
        public void Find_WithPredicate_ShouldReturnMatchingTemplates()
        {
            _context.Templates.AddRange(
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Match" },
                new Domain.Model.Template { Id = Guid.NewGuid(), Title = "Skip" }
            );
            _context.SaveChanges();

            var result = _repository.Find(t => t.Title == "Match");

            Assert.Single(result);
            Assert.Equal("Match", result.First().Title);
        }
    }
}