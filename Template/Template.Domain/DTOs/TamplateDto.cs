using System;
using System.Collections.Generic;

namespace Template.Domain.DTOs
{
    public record TamplateDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public UserDto? Owner { get; init; }
        public TopicDto? Topic { get; init; }
        public IEnumerable<TagDto>? Tags { get; init; }
    }
}
