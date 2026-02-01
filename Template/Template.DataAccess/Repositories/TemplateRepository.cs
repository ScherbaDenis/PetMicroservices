using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Template entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class TemplateRepository(TemplateDbContext context, ILogger<TemplateRepository> logger) 
        : RepositoryBase<Domain.Model.Template, Guid>(context, logger), ITemplateRepository
    {
        /// <inheritdoc/>
        public async Task<IEnumerable<Domain.Model.Template>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Owner)
                .Include(t => t.Topic)
                .Include(t => t.Tags)
                .Include(t => t.UsersAccess)
                .Include(t => t.Questions)
                .Where(t => t.Owner != null && t.Owner.Id == userId || 
                           t.UsersAccess != null && t.UsersAccess.Any(u => u.Id == userId))
                .ToListAsync(cancellationToken);
        }
    }
}
