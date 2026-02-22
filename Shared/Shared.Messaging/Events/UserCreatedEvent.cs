namespace Shared.Messaging.Events;

public record UserCreatedEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
