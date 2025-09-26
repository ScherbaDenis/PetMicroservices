using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly ILogger<UserService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task CreateAsync(User item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating user: {User}", item);

            await _userRepository.AddAsync(item, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created successfully: {User}", item);
        }

        public async Task DeleteAsync(User item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting user: {User}", item);

            await _userRepository.DeleteAsync(item, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User deleted successfully: {User}", item);
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            _logger.LogInformation("Finding user...");

            return _userRepository.Find(predicate);
        }

        public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding user...");

            var user = await _userRepository.FindAsync(id, cancellationToken);

            if (user == null)
                _logger.LogWarning("No user found");
            else
                _logger.LogInformation("User found: {User}", user);

            return user;
        }

        public IEnumerable<User> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all users...");

            var users = _userRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} users", users is ICollection<User> col ? col.Count : -1);

            return users;
        }

        public async Task UpdateAsync(User item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating user: {User}", item);

            await _userRepository.UpdateAsync(item, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User updated successfully: {User}", item);
        }
    }
}
