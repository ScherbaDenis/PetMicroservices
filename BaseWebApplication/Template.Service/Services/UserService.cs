using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

    }
}
