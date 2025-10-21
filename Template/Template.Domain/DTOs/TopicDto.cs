namespace Template.Domain.DTOs
{
    public record TopicDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
