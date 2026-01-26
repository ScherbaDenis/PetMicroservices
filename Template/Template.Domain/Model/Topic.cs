using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public class Topic : Entity<int>
    {
        public string Name { get; set; }
    }
}
