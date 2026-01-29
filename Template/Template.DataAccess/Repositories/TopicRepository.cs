using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Topic entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class TopicRepository(TemplateDbContext context, ILogger<TopicRepository> logger) 
        : RepositoryBase<Topic, int>(context, logger), ITopicRepository
    {
    }
}