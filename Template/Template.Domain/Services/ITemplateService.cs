using Template.Domain.DTOs;

namespace Template.Domain.Services
{
    public interface ITemplateService : IService<TemplateDto, Guid>
    {
        /// <summary>
        /// Asynchronously retrieves all templates associated with a specific user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of template DTOs associated with the user.</returns>
        Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
