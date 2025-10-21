namespace Template.Domain.DTOs
{
    public record TagDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
