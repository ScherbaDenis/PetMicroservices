using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class TagService(ITagRepository tagRepository) : ITagService
    {
        private readonly ITagRepository tagRepository = tagRepository;

        public void CreateAsync(Tag item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(Tag item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Tag?> FindAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync(Tag item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
