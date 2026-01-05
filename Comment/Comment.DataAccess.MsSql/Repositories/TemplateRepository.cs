using Microsoft.Extensions.Logging;
using Comment.Domain.Models;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    public class TemplateRepository(CommentDbContext context, ILogger<TemplateRepository> logger) : ITemplateRepository
    {
        private readonly CommentDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<TemplateRepository> _logger = logger ?? throw new ArgumentNullException(nameof(context));

        public async Task AddAsync(Template item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Adding a new Template: {Template}", item);
            await _context.Templates.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(Template item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting Template: {Template}", item);
            _context.Templates.Remove(item);
            await Task.CompletedTask; // keep async signature
        }

        public async Task<Template?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Async Finding a Template...");
            return await _context.Templates.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Template> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all Templates...");
            return _context.Templates.ToList();
        }

        public async Task UpdateAsync(Template item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating Template: {Template}", item);
            _context.Templates.Update(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database...");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public IEnumerable<Template> Find(Func<Template, bool> predicate)
        {
            _logger.LogInformation("Finding a Template...");
            return _context.Templates.Where(predicate);
        }
    }
}
