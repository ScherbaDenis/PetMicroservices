using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Repository for managing Question entities.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public class QuestionRepository(TemplateDbContext context, ILogger<QuestionRepository> logger) 
        : RepositoryBase<Question, Guid>(context, logger), IQuestionRepository
    {
    }
}
