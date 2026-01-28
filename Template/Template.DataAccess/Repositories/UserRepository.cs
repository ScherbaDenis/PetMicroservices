using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing User entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class UserRepository(TemplateDbContext context, ILogger<UserRepository> logger) 
        : RepositoryBase<User, Guid>(context, logger), IUserRepository
    {
    }
}
