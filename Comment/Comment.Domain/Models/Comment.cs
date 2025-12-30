using Comment.Domain.Repositories;

namespace Comment.Domain.Models
{
    public class Comment : Entity<Guid>
    {
        public Guid Id { get; set; }

        public Tamplate Tamplate { get; set; }

        public string Text { get; set; }

    }
}
