using Comment.Domain.Repositories;

namespace Comment.Domain.Models
{
    public class Tamplate : Entity<Guid>
    {
        public string Title { get; set; }

    }
}
