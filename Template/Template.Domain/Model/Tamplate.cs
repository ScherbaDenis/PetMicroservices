using System;
using System.Collections.Generic;

namespace Template.Domain.Model
{
    public class Tamplate : Entity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public User? Owner { get; set; }
        public Topic? Topic { get; set; }
        public ICollection<Tag>? Tags { get; set; }
        public ICollection<Question>? Questions { get; set; }
    }
}
