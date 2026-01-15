using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TopicService : ITopicService
    {
        public Task<TopicDto> CreateAsync(TopicDto item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TopicDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TopicDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TopicDto item, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
