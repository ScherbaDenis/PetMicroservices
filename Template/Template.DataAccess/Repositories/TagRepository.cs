using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Tag entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class TagRepository(TemplateDbContext context, ILogger<TagRepository> logger) 
        : RepositoryBase<Tag, int>(context, logger), ITagRepository
    {
    }
}
