using Microsoft.EntityFrameworkCore;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly TamplateDbContext _context;

        public TagRepository(TamplateDbContext context)
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

        public async void CreateAsync(Tag item, CancellationToken cancellationToken = default)
        {
            await _context.Tags.AddAsync(item, cancellationToken);
        }

        public void Delete(int id)
        {
            var item = _context.Tags.FirstOrDefault(x => x.Id == id);
            if (item != null)
                _context.Remove(item);
        }

        public IEnumerable<Tag> Find(Func<Tag, bool> predicate)
        {
            return _context.Tags.Where(predicate);
        }

        public IEnumerable<Tag> GetAll()
        {
            return _context.Tags;
        }

        public async Task<Tag?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tags.FindAsync(id, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(Tag item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
