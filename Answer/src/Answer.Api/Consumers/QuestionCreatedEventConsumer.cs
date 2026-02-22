using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using MassTransit;
using Shared.Messaging.Events;

namespace Answer.Api.Consumers;

public class QuestionCreatedEventConsumer(IRepository<Question> questionRepository, ILogger<QuestionCreatedEventConsumer> logger)
    : IConsumer<QuestionCreatedEvent>
{
    private readonly IRepository<Question> _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
    private readonly ILogger<QuestionCreatedEventConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Consume(ConsumeContext<QuestionCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received QuestionCreatedEvent for question: {QuestionId}", message.Id);

        var existing = await _questionRepository.GetByIdAsync(message.Id);
        if (existing != null)
        {
            _logger.LogInformation("Question {QuestionId} already exists in Answer service, skipping", message.Id);
            return;
        }

        var question = new Question
        {
            Id = message.Id,
            Title = message.Title
        };

        await _questionRepository.AddAsync(question);

        _logger.LogInformation("Question {QuestionId} created in Answer service", message.Id);
    }
}
