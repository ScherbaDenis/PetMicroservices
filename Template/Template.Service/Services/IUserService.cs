using Template.Domain.DTOs;

namespace Template.Service.Services
{
    public interface IUserService : IService<UserDto, Guid>
    {
    }
}
