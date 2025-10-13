using Microsoft.Extensions.Logging;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    public class UnitOfWork(
        TamplateDbContext context,
        ILogger<UnitOfWork> logger,
        ILoggerFactory loggerFactory) : IUnitOfWork
    {
        private readonly TamplateDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<UnitOfWork> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

        private ITamplateRepository _tamplateRepository;
        private ITopicRepository _topicRepository;
        private IUserRepository _userRepository;
        private ITagRepository _tagRepository;

        private bool _disposed;

        public ITamplateRepository TamplateRepository =>
            _tamplateRepository ??= new TamplateRepository(
                _context,
                _loggerFactory.CreateLogger<TamplateRepository>());

        public ITopicRepository TopicRepository =>
            _topicRepository ??= new TopicRepository(
                _context,
                _loggerFactory.CreateLogger<TopicRepository>());

        public IUserRepository UserRepository =>
            _userRepository ??= new UserRepository(
                _context,
                _loggerFactory.CreateLogger<UserRepository>());

        public ITagRepository TagRepository =>
            _tagRepository ??= new TagRepository(
                _context,
                _loggerFactory.CreateLogger<TagRepository>());

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