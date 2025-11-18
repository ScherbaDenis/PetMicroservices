namespace Comment.Domain.Repositories
{
    public abstract class Entity<T>
    {
        public T Id { get; set; }  
    }
}
