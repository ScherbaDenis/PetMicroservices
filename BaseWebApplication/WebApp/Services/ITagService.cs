using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface ITagService
    {
        Task<TagDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<TagDto> CreateAsync(TagDto item, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(TagDto item, CancellationToken cancellationToken);
        Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken);
    }
}
