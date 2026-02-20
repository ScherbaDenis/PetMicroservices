using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using MassTransit;
using Shared.Messaging.Events;

namespace Answer.Api.Consumers;

public class UserCreatedEventConsumer(IRepository<User> userRepository, ILogger<UserCreatedEventConsumer> logger)
    : IConsumer<UserCreatedEvent>
{
    private readonly IRepository<User> _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly ILogger<UserCreatedEventConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received UserCreatedEvent for user: {UserId}", message.Id);

        var existing = await _userRepository.GetByIdAsync(message.Id);
        if (existing != null)
        {
            _logger.LogInformation("User {UserId} already exists in Answer service, skipping", message.Id);
            return;
        }

        var user = new User
        {
            Id = message.Id,
            Name = message.Name
        };

        await _userRepository.AddAsync(user);

        _logger.LogInformation("User {UserId} created in Answer service", message.Id);
    }
}
