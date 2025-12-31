using Comment.Domain.Repositories;

namespace Comment.Domain.Models
{
    public class Template : Entity<Guid>
    {
        public string Title { get; set; }

    }
}
