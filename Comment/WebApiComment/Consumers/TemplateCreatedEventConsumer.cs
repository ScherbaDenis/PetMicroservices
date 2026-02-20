using Comment.Domain.DTOs;
using Comment.Domain.Services;
using MassTransit;
using Shared.Messaging.Events;

namespace WebApiComment.Consumers;

public class TemplateCreatedEventConsumer(ITemplateService templateService, ILogger<TemplateCreatedEventConsumer> logger)
    : IConsumer<TemplateCreatedEvent>
{
    private readonly ITemplateService _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
    private readonly ILogger<TemplateCreatedEventConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Consume(ConsumeContext<TemplateCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TemplateCreatedEvent for template: {TemplateId}", message.Id);

        var existing = await _templateService.FindAsync(message.Id, context.CancellationToken);
        if (existing != null)
        {
            _logger.LogInformation("Template {TemplateId} already exists in Comment service, skipping", message.Id);
            return;
        }

        await _templateService.CreateAsync(new TemplateDto
        {
            Id = message.Id,
            Title = message.Title
        }, context.CancellationToken);

        _logger.LogInformation("Template {TemplateId} created in Comment service", message.Id);
    }
}
