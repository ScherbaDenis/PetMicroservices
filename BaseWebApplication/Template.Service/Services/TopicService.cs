using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class TopicService(ITopicRepository topicRepository) : ITopicService
    {
        private readonly ITopicRepository topicRepository = topicRepository;

        public void CreateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(Topic item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Topic?> FindAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Topic> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
