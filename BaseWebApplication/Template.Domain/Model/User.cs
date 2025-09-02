using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public class User : Entity<Guid>
    {
        public required string Name { get; set; }
    }
}
