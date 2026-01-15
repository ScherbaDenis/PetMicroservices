using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class UserService : IUserService
    {
        public Task<UserDto> CreateAsync(UserDto item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserDto item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
