using Template.Domain.DTOs;

namespace Template.Domain.Services
{
    public interface IUserService : IService<UserDto, Guid>
    {
    }
}
