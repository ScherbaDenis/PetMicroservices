namespace WebApp.Services.DTOs
{
    public record TopicDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
