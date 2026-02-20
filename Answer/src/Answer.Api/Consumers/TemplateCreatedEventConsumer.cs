using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using MassTransit;
using Shared.Messaging.Events;

namespace Answer.Api.Consumers;

public class TemplateCreatedEventConsumer(IRepository<Template> templateRepository, ILogger<TemplateCreatedEventConsumer> logger)
    : IConsumer<TemplateCreatedEvent>
{
    private readonly IRepository<Template> _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
    private readonly ILogger<TemplateCreatedEventConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Consume(ConsumeContext<TemplateCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TemplateCreatedEvent for template: {TemplateId}", message.Id);

        var existing = await _templateRepository.GetByIdAsync(message.Id);
        if (existing != null)
        {
            _logger.LogInformation("Template {TemplateId} already exists in Answer service, skipping", message.Id);
            return;
        }

        var template = new Template
        {
            Id = message.Id,
            Title = message.Title
        };

        await _templateRepository.AddAsync(template);

        _logger.LogInformation("Template {TemplateId} created in Answer service", message.Id);
    }
}
