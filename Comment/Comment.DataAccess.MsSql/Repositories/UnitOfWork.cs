using Microsoft.Extensions.Logging;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    public class UnitOfWork(
        CommentDbContext context,
        ILogger<UnitOfWork> logger,
        ILoggerFactory loggerFactory) : IUnitOfWork
    {
        private readonly CommentDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<UnitOfWork> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        private ITemplateRepository _templateRepository;
        private ICommentRepository _commentRepository;

        private bool _disposed;

        public ITemplateRepository TemplateRepository =>
            _templateRepository ??= new TemplateRepository(
                _context,
                _loggerFactory.CreateLogger<TemplateRepository>());

        public ICommentRepository CommentRepository =>
            _commentRepository ??= new CommentRepository(
                _context,
                _loggerFactory.CreateLogger<CommentRepository>());

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes in UnitOfWork");
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}