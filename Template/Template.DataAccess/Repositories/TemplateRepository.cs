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
            // Load all templates with their relationships
            var allTemplates = await _dbSet
                .Include(t => t.Owner)
                .Include(t => t.Topic)
                .Include(t => t.Tags)
                .Include(t => t.UsersAccess)
                .Include(t => t.Questions)
                .ToListAsync(cancellationToken);

            // Filter in-memory to support both real DB and in-memory DB
            return allTemplates.Where(t => 
                (t.Owner != null && t.Owner.Id == userId) || 
                (t.UsersAccess != null && t.UsersAccess.Any(u => u.Id == userId))
            );
        }

        /// <inheritdoc/>
        public async Task AssignTemplateToUserAsync(Guid templateId, Guid userId, CancellationToken cancellationToken = default)
        {
            var template = await _dbSet
                .Include(t => t.UsersAccess)
                .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

            if (template == null)
            {
                throw new InvalidOperationException($"Template with Id {templateId} not found.");
            }

            // Get the user from the context (cast to TemplateDbContext)
            var dbContext = (TemplateDbContext)_context;
            var user = await dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with Id {userId} not found.");
            }

            // Check if user is already assigned
            if (template.UsersAccess != null && template.UsersAccess.Any(u => u.Id == userId))
            {
                return; // User already has access
            }

            // Add user to UsersAccess
            if (template.UsersAccess == null)
            {
                template.UsersAccess = new List<Domain.Model.User>();
            }
            ((List<Domain.Model.User>)template.UsersAccess).Add(user);
        }

        /// <inheritdoc/>
        public async Task UnassignTemplateFromUserAsync(Guid templateId, Guid userId, CancellationToken cancellationToken = default)
        {
            var template = await _dbSet
                .Include(t => t.UsersAccess)
                .FirstOrDefaultAsync(t => t.Id == templateId, cancellationToken);

            if (template == null)
            {
                throw new InvalidOperationException($"Template with Id {templateId} not found.");
            }

            if (template.UsersAccess == null)
            {
                return; // No users assigned
            }

            var userToRemove = template.UsersAccess.FirstOrDefault(u => u.Id == userId);
            if (userToRemove != null)
            {
                ((List<Domain.Model.User>)template.UsersAccess).Remove(userToRemove);
            }
        }
    }
}
