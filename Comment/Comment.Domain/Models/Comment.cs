using Comment.Domain.Repositories;

namespace Comment.Domain.Models
{
    public class Comment : Entity<Guid>
    {
        public Guid? TemplateId { get; set; }

        public Template Template { get; set; }

        public string Text { get; set; }

    }
}
