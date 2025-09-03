using Microsoft.EntityFrameworkCore;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    public class TamplateRepository : ITamplateRepository
    {
        private readonly TamplateDbContext _context;

        public TamplateRepository(TamplateDbContext context)
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

        public async void CreateAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            await _context.Tamplates.AddAsync(item, cancellationToken);
        }

        public void Delete(Guid id)
        {
            var item = _context.Tamplates.FirstOrDefault(x => x.Id == id);
            if (item != null)
                _context.Remove(item);
        }

        public IEnumerable<Tamplate> Find(Func<Tamplate, bool> predicate)
        {
            return _context.Tamplates.Where(predicate);
        }

        public async Task<Tamplate?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Tamplates.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Tamplate> GetAll()
        {
            return _context.Tamplates;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(Tamplate item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
