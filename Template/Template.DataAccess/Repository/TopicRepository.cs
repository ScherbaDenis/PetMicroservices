using Microsoft.EntityFrameworkCore;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    internal class TopicRepository : ITopicRepository
    {
        private readonly TamplateDbContext _context;

        public TopicRepository(TamplateDbContext context)
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

        public async void CreateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            await _context.Topics.AddAsync(item, cancellationToken);
        }

        public void Delete(int id)
        {
            var item = _context.Topics.FirstOrDefault(x => x.Id == id);
            if (item != null)
                _context.Remove(item);
        }

        public IEnumerable<Topic> Find(Func<Topic, bool> predicate)
        {
            return _context.Topics.Where(predicate);
        }

        public async Task<Topic?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Topics.FindAsync(id, cancellationToken);
        }

        public IEnumerable<Topic> GetAll()
        {
            return _context.Topics;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(Topic item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
