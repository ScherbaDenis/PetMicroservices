using Template.Domain.Model;

namespace Template.Domain.Repository
{
    public interface ITemplateRepository : IRepository<Model.Template, Guid>
    {
        /// <summary>
        /// Asynchronously retrieves all templates associated with a specific user.
        /// This includes templates where the user is the owner or has access.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of templates associated with the user.</returns>
        Task<IEnumerable<Model.Template>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
