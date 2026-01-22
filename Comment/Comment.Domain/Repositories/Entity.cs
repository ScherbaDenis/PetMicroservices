namespace Comment.Domain.Repositories
{
    /// <summary>
    /// Base class for entities with an identifier.
    /// </summary>
    /// <typeparam name="T">The type of the entity's identifier.</typeparam>
    public abstract class Entity<T>
    {
        /// <summary>
        /// Unique identifier of the entity.
        /// </summary>
        public T Id { get; set; }  
    }
}
