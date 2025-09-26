using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    public class UserRepository(TamplateDbContext context, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly TamplateDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<UserRepository> _logger = logger ?? throw new ArgumentNullException(nameof(context));

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public async Task AddAsync(User item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Adding a new User: {User}", item);
            await _context.Users.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(User item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting User: {User}", item);
            _context.Users.Remove(item);
            await Task.CompletedTask; // keep async signature
        }

        public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Async Finding a User...");
            return await _context.Users.FindAsync(id, cancellationToken);
        }

        public IEnumerable<User> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all Users...");
            return _context.Users.ToList();
        }

        public async Task UpdateAsync(User item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating User: {User}", item);
            _context.Users.Update(item);
            await Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Saving changes to database...");
            return await _context.SaveChangesAsync(cancellationToken);
        }
        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            _logger.LogInformation("Finding a User...");
            return _context.Users.Where(predicate);
        }
    }
}
