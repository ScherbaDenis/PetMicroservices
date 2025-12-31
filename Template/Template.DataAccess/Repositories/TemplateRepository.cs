using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    public class TemplateRepository(TemplateDbContext context, ILogger<TemplateRepository> logger) : ITemplateRepository
    {
        private readonly TemplateDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<TemplateRepository> _logger = logger ?? throw new ArgumentNullException(nameof(context));

        public async Task AddAsync(Domain.Model.Template item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Adding a new Tamplate: {Tamplate}", item);
            await _context.Tamplates.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(Domain.Model.Template item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting Tamplate: {Tamplate}", item);
            _context.Tamplates.Remove(item);
            await Task.CompletedTask; // keep async signature
        }

        public async Task<Domain.Model.Template?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Async Finding a Tamplate...");
            return await _context.Tamplates.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Domain.Model.Template> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all Tamplates...");
            return _context.Tamplates.ToList();
        }

        public async Task UpdateAsync(Domain.Model.Template item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating Tamplate: {Tamplate}", item);
            _context.Tamplates.Update(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database...");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public IEnumerable<Domain.Model.Template> Find(Func<Domain.Model.Template, bool> predicate)
        {
            _logger.LogInformation("Finding a Tamplate...");
            return _context.Tamplates.Where(predicate);
        }
    }
}
