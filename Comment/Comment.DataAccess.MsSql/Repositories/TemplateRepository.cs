using Microsoft.Extensions.Logging;
using Comment.Domain.Models;
using Comment.Domain.Repositories;

namespace Comment.DataAccess.MsSql.Repositories
{
    public class TemplateRepository(CommentDbContext context, ILogger<TemplateRepository> logger)
       : RepositoryBase<Template, Guid>(context, logger), ITemplateRepository
    {
    }
}
