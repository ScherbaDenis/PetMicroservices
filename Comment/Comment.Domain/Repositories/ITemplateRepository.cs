using Comment.Domain.Models;

namespace Comment.Domain.Repositories
{
    /// <summary>
    /// Repository for managing Template entities.
    /// </summary>
    public interface ITemplateRepository : IRepository<Template, Guid>
    {
    }
}
