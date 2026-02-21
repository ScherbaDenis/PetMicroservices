namespace Shared.Messaging.Events;

public record QuestionCreatedEvent
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
}
