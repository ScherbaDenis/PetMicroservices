using Microsoft.EntityFrameworkCore;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly TamplateDbContext _context;

        public UserRepository(TamplateDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public async void CreateAsync(User item, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(item, cancellationToken);
        }

        public void Delete(Guid id)
        {
            var item = _context.Users.FirstOrDefault(x => x.Id == id);
            if (item != null)
                _context.Remove(item);
        }

        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            return _context.Users.Where(predicate);
        }

        public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FindAsync(id, cancellationToken);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(User item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
