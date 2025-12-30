using Microsoft.Extensions.Logging;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    public class CommentRepository(CommentDbContext context, ILogger<CommentRepository> logger) : ICommentRepository
    {
        private readonly CommentDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<CommentRepository> _logger = logger ?? throw new ArgumentNullException(nameof(context));

        public async Task AddAsync(Domain.Models.Comment item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Adding a new comment: {Comment}", item);
            await _context.Comments.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(Domain.Models.Comment item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting comment: {Comment}", item);
            _context.Comments.Remove(item);
            await Task.CompletedTask; // keep async signature
        }

        public async Task<Domain.Models.Comment?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Async Finding a comment...");
            return await _context.Comments.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Domain.Models.Comment> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all comments...");
            return [.. _context.Comments];
        }

        public async Task UpdateAsync(Domain.Models.Comment item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating comment: {Comment}", item);
            _context.Comments.Update(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database...");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public IEnumerable<Domain.Models.Comment> Find(Func<Domain.Models.Comment, bool> predicate)
        {
            _logger.LogInformation("Finding a comment...");
            return _context.Comments.Where(predicate);
        }
    }
}