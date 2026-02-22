using System;
using System.Collections.Generic;

namespace Template.Domain.DTOs
{
    public record TemplateDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid? OwnerId { get; init; }
        public UserDto? Owner { get; init; }
        public int? TopicId { get; init; }
        public TopicDto? Topic { get; init; }
        public IEnumerable<TagDto>? Tags { get; init; }
        public IEnumerable<QuestionDto>? Questions { get; init; }
        public IEnumerable<UserDto>? UsersAccess { get; init; }
    }
}
