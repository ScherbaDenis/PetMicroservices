namespace Shared.Messaging.Events;

public record TemplateCreatedEvent
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
}
