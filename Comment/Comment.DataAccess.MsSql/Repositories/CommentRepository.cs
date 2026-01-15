using Microsoft.Extensions.Logging;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    public class CommentRepository(CommentDbContext context, ILogger<CommentRepository> logger) 
        : RepositoryBase<Domain.Models.Comment, Guid>(context, logger), ICommentRepository
    {
    }
}