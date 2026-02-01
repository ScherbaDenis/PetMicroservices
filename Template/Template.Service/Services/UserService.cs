using System.Linq;
using Template.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Service.Mappers;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<UserService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUserRepository _userRepository = unitOfWork.UserRepository;

    // Use centralized mappers
    

        public async Task CreateAsync(UserDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating user: {User}", item);

            var entity = item.ToEntity();
            await _userRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created successfully: {User}", entity);
        }

        public async Task DeleteAsync(UserDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting user: {User}", item);

            var entity = await _userRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"User with Id {item.Id} not found.");

            await _userRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User deleted successfully: {User}", entity);
        }

        public async Task<IEnumerable<UserDto>> FindAsync(Func<UserDto, bool> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding users with predicate...");

            var entities = await _userRepository.GetAllAsync(cancellationToken);
            var dtos = entities.Select(e => e.ToDto()).Where(predicate);
            return dtos;
        }

        public async Task<UserDto?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding user...");

            var user = await _userRepository.FindAsync(id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("No user found");
                return null;
            }

            _logger.LogInformation("User found: {User}", user);
            return user.ToDto();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all users...");

            var users = await _userRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} users", users is ICollection<User> col ? col.Count : -1);

            return users.Select(u => u.ToDto());
        }

        public async Task UpdateAsync(UserDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating user: {User}", item);

            var entity = await _userRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"User with Id {item.Id} not found.");
            entity.Name = item.Name; // Todo Update other properties as needed

            await _userRepository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User updated successfully: {User}", entity);
        }
    }
}
