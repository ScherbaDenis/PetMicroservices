using Template.Domain.Model;

namespace Template.Domain.Repository
{
    public interface IUserRepository : IRepository<User, Guid>
    {
    }
}
