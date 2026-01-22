namespace Comment.Domain.Repositories
{
    /// <summary>
    /// Repository interface for managing Comment entities.
    /// </summary>
    public interface ICommentRepository : IRepository<Models.Comment, Guid>
    {
    }
}
