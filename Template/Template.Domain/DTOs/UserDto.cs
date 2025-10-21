using System;

namespace Template.Domain.DTOs
{
    public record UserDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
