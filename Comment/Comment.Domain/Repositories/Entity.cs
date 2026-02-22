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

        /// <summary>
        /// Date and time when the entity was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time when the entity was last updated.
        /// </summary>
        public DateTime DateUpdated { get; set; }

        /// <summary>
        /// Indicates whether the entity has been soft deleted.
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
