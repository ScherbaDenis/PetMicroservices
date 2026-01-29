using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<UserDto> CreateAsync(UserDto item, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateAsync(UserDto item, CancellationToken cancellationToken);
        Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken);
    }
}
