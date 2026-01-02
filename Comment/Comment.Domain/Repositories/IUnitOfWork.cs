namespace Comment.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITemplateRepository TemplateRepository { get; }
        ICommentRepository  CommentRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
