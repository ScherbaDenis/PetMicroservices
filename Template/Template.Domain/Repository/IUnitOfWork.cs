namespace Template.Domain.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ITamplateRepository TamplateRepository { get; }
        ITopicRepository TopicRepository { get; }
        IUserRepository UserRepository { get; }
        ITagRepository TagRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
