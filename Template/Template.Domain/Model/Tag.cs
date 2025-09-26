using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public class Tag : Entity<int>
    {
        public string Name { get; set; }
    }
}
