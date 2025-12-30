using System;

namespace Template.Domain.Model
{
    public class Question : Entity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
