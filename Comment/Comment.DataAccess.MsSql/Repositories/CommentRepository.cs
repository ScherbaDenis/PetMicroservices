using Microsoft.Extensions.Logging;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Comment entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class CommentRepository(CommentDbContext context, ILogger<CommentRepository> logger) 
        : RepositoryBase<Domain.Models.Comment, Guid>(context, logger), ICommentRepository
    {
    }
}