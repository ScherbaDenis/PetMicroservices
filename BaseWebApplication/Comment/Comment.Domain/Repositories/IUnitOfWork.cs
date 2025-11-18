namespace Comment.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ITamplateRepository TamplateRepository { get; }
        ICommentRepository  CommentRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
