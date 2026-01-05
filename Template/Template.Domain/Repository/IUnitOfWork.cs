namespace Template.Domain.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ITemplateRepository TemplateRepository { get; }
        ITopicRepository TopicRepository { get; }
        IUserRepository UserRepository { get; }
        ITagRepository TagRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
