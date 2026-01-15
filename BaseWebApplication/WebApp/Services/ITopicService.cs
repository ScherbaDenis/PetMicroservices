using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface ITopicService
    {
        Task<TopicDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<TopicDto> CreateAsync(TopicDto item, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(TopicDto item, CancellationToken cancellationToken);
        Task<IEnumerable<TopicDto>> GetAllAsync(CancellationToken cancellationToken);
    }
}
