using Comment.Domain.Repositories;

namespace Comment.Domain.Models
{
    public class Comment : Entity<Guid>
    {
        public Template Template { get; set; }

        public string Text { get; set; }

    }
}
