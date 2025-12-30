using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;
using Template.Service.Mappers;

namespace Template.Service.Services
{
    public class QuestionService(IUnitOfWork unitOfWork, ILogger<QuestionService> logger) : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<QuestionService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IQuestionRepository _questionRepository = unitOfWork.QuestionRepository;

        public async Task CreateAsync(QuestionDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Creating question: {Question}", item);
            var entity = item.ToEntity();
            await _questionRepository.AddAsync(entity, cancellationToken);
            await _questionRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Question created successfully: {Question}", entity);
        }

        public async Task UpdateAsync(QuestionDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating question: {Question}", item);
            var entity = item.ToEntity();
            await _questionRepository.UpdateAsync(entity, cancellationToken);
            await _questionRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Question updated successfully: {Question}", entity);
        }

        public async Task DeleteAsync(QuestionDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Deleting question: {Question}", item);
            var entity = item.ToEntity();
            await _questionRepository.DeleteAsync(entity, cancellationToken);
            await _questionRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Question deleted successfully: {Question}", entity);
        }

        public IEnumerable<QuestionDto> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all questions...");
            var questions = _questionRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} questions", questions is ICollection<Question> col ? col.Count : -1);
            return questions.Select(q => q.ToDto());
        }

        public async Task<QuestionDto?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding question...");
            var question = await _questionRepository.FindAsync(id, cancellationToken);
            if (question == null)
            {
                _logger.LogWarning("No question found");
                return null;
            }
            _logger.LogInformation("Question found: {Question}", question);
            return question.ToDto();
        }

        public IEnumerable<QuestionDto> Find(Func<QuestionDto, bool> predicate)
        {
            _logger.LogInformation("Finding question...");
            var entities = _questionRepository.Find(q => predicate(q.ToDto()));
            return entities.Select(q => q.ToDto());
        }
    }
}
