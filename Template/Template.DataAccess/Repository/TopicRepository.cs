using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    public class TopicRepository(TamplateDbContext context, ILogger<TopicRepository> logger) : ITopicRepository
    {
        private readonly TamplateDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<TopicRepository> _logger = logger ?? throw new ArgumentNullException(nameof(context));

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public async Task AddAsync(Topic item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Adding a new topic: {Topic}", item);
            await _context.Topics.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(Topic item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Deleting topic: {Topic}", item);
            _context.Topics.Remove(item);
            await Task.CompletedTask; // keep async signature
        }

        public async Task<Topic?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Async Finding a topic...");
            return await _context.Topics.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Topic> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all topics...");
            return [.. _context.Topics];
        }

        public async Task UpdateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Updating topic: {Topic}", item);
            _context.Topics.Update(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database...");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public IEnumerable<Topic> Find(Func<Topic, bool> predicate)
        {
            _logger.LogInformation("Finding a topic...");
            return _context.Topics.Where(predicate);
        }
    }
}