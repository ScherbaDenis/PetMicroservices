using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Comment entities, including related Template.
    /// </summary>
    public class CommentRepository : RepositoryBase<Domain.Models.Comment, Guid>, ICommentRepository
    {
        private readonly CommentDbContext _commentContext;

        public CommentRepository(CommentDbContext context, ILogger<CommentRepository> logger)
            : base(context, logger)
        {
            _commentContext = context;
        }

        /// <summary>
        /// Gets all comments including their related Template, excluding soft-deleted items.
        /// </summary>
        public override async Task<IEnumerable<Domain.Models.Comment>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _commentContext.Comments
                .Include(c => c.Template)
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Finds a comment by id including its related Template, excluding soft-deleted items.
        /// </summary>
        public override async Task<Domain.Models.Comment?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _commentContext.Comments
                .Include(c => c.Template)
                .Where(c => !c.IsDeleted)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        /// <summary>
        /// Finds comments matching a predicate, including their related Template, excluding soft-deleted items.
        /// </summary>
        public override async Task<IEnumerable<Domain.Models.Comment>> FindAsync(
            System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _commentContext.Comments
                .Include(c => c.Template)
                .Where(c => !c.IsDeleted)
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets a paged list of comments including their related Template, excluding soft-deleted items.
        /// </summary>
        public override async Task<(IEnumerable<Domain.Models.Comment> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            System.Linq.Expressions.Expression<Func<Domain.Models.Comment, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            var query = _commentContext.Comments.Include(c => c.Template).Where(c => !c.IsDeleted).AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (items, totalCount);
        }
    }
}
