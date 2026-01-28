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
    }
}
