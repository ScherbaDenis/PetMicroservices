using Microsoft.Extensions.Logging;
using Comment.Domain.Models;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Template entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class TemplateRepository(CommentDbContext context, ILogger<TemplateRepository> logger)
       : RepositoryBase<Template, Guid>(context, logger), ITemplateRepository
    {
    }
}
