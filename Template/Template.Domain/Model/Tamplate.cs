using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public class Tamplate : Entity<Guid>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public User Owner { get; set; }

        public Topic Topic { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public IEnumerable<User> UsersAccess { get; set; }

    }
}
