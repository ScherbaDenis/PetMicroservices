namespace Comment.Domain.Repositories
{
    /// <summary>
    /// Unit of Work interface for managing repositories and committing changes.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Template repository.
        /// </summary>
        ITemplateRepository TemplateRepository { get; }
        /// <summary>
        /// Comment repository.
        /// </summary>
        ICommentRepository CommentRepository { get; }
        /// <summary>
        /// Commits all changes made in the unit of work to the data store.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
