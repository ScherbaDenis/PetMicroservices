using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository userRepository = userRepository;

        public void CreateAsync(User item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(User item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> FindAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync(User item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
