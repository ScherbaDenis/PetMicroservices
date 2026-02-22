namespace Template.Domain.Repository
{
    public abstract class Entity<T>
    {
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
