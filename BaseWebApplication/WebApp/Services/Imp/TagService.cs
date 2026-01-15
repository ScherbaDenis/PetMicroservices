using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TagService : ITagService
    {
        public Task<TagDto> CreateAsync(TagDto item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TagDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TagDto item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
