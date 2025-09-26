using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public class User : Entity<Guid>
    {
        public string Name { get; set; }
    }
}
