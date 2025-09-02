namespace Template.Domain.Repository
{
    public abstract class Entity<T>
    {
        public required T Id { get; set; }  
    }
}
