using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    public class TagRepository(TemplateDbContext context, ILogger<TagRepository> logger) : ITagRepository
    {
        private readonly TemplateDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<TagRepository> _logger = logger ?? throw new ArgumentNullException(nameof(context));

        public async Task AddAsync(Tag item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Adding a new Tag: {Tag}", item);
            await _context.Tags.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(Tag item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting Tag: {Tag}", item);
            _context.Tags.Remove(item);
            await Task.CompletedTask; // keep async signature
        }

        public async Task<Tag?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Async Finding a Tag...");
            return await _context.Tags.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Tag> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all Tags...");
            return [.. _context.Tags];
        }

        public async Task UpdateAsync(Tag item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating Tag: {Tag}", item);
            _context.Tags.Update(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database...");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public IEnumerable<Tag> Find(Func<Tag, bool> predicate)
        {
            _logger.LogInformation("Finding a Tag...");
            return _context.Tags.Where(predicate);
        }
    }
}
