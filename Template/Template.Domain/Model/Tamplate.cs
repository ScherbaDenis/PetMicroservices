using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public class Tamplate : Entity<Guid>
    {
        public required string Title { get; set; }

        public required string Description { get; set; }

        public required User Owner { get; set; }

        public required Topic Topic { get; set; }

        public required IEnumerable<Tag> Tags { get; set; }

        public required IEnumerable<User> UsersAccess { get; set; }

    }
}
