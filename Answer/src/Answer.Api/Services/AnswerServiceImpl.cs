using Answer.Api.Protos;
using Answer.Application.Interfaces;
using Grpc.Core;

namespace Answer.Api.Services;

public class AnswerServiceImpl : AnswerService.AnswerServiceBase
{
    private readonly IRepository<Domain.Entities.Answer> _answerRepository;
    private readonly IRepository<Domain.Entities.User> _userRepository;
    private readonly IRepository<Domain.Entities.Question> _questionRepository;
    private readonly IRepository<Domain.Entities.Template> _templateRepository;

    public AnswerServiceImpl(
        IRepository<Domain.Entities.Answer> answerRepository,
        IRepository<Domain.Entities.User> userRepository,
        IRepository<Domain.Entities.Question> questionRepository,
        IRepository<Domain.Entities.Template> templateRepository)
    {
        _answerRepository = answerRepository;
        _userRepository = userRepository;
        _questionRepository = questionRepository;
        _templateRepository = templateRepository;
    }

    public override async Task<AnswerResponse> GetAnswer(GetAnswerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid answer ID format"));
        }

        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Answer not found"));
        }

        var user = await _userRepository.GetByIdAsync(answer.UserId);
        var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
        var template = await _templateRepository.GetByIdAsync(answer.TemplateId);

        return new AnswerResponse
        {
            Id = answer.Id.ToString(),
            UserId = answer.UserId.ToString(),
            UserName = user?.Name ?? string.Empty,
            QuestionId = answer.QuestionId.ToString(),
            QuestionTitle = question?.Title ?? string.Empty,
            TemplateId = answer.TemplateId.ToString(),
            TemplateTitle = template?.Title ?? string.Empty,
            AnswerType = MapAnswerType(answer.AnswerType),
            AnswerValue = answer.AnswerValue
        };
    }

    public override async Task<ListAnswersResponse> ListAnswers(ListAnswersRequest request, ServerCallContext context)
    {
        var answers = await _answerRepository.GetAllAsync();
        var response = new ListAnswersResponse();
        
        // Note: This has an N+1 query pattern. For a production scenario with a real database,
        // consider implementing eager loading or batch fetching of related entities.
        // For this in-memory demonstration, the performance impact is negligible.
        foreach (var answer in answers)
        {
            var user = await _userRepository.GetByIdAsync(answer.UserId);
            var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
            var template = await _templateRepository.GetByIdAsync(answer.TemplateId);

            response.Answers.Add(new AnswerResponse
            {
                Id = answer.Id.ToString(),
                UserId = answer.UserId.ToString(),
                UserName = user?.Name ?? string.Empty,
                QuestionId = answer.QuestionId.ToString(),
                QuestionTitle = question?.Title ?? string.Empty,
                TemplateId = answer.TemplateId.ToString(),
                TemplateTitle = template?.Title ?? string.Empty,
                AnswerType = MapAnswerType(answer.AnswerType),
                AnswerValue = answer.AnswerValue
            });
        }

        return response;
    }

    public override async Task<AnswerResponse> CreateAnswer(CreateAnswerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));
        }

        if (!Guid.TryParse(request.QuestionId, out var questionId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid question ID format"));
        }

        if (!Guid.TryParse(request.TemplateId, out var templateId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid template ID format"));
        }

        var answer = new Domain.Entities.Answer
        {
            UserId = userId,
            QuestionId = questionId,
            TemplateId = templateId,
            AnswerType = MapAnswerType(request.AnswerType),
            AnswerValue = request.AnswerValue
        };

        await _answerRepository.AddAsync(answer);

        var user = await _userRepository.GetByIdAsync(answer.UserId);
        var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
        var template = await _templateRepository.GetByIdAsync(answer.TemplateId);

        return new AnswerResponse
        {
            Id = answer.Id.ToString(),
            UserId = answer.UserId.ToString(),
            UserName = user?.Name ?? string.Empty,
            QuestionId = answer.QuestionId.ToString(),
            QuestionTitle = question?.Title ?? string.Empty,
            TemplateId = answer.TemplateId.ToString(),
            TemplateTitle = template?.Title ?? string.Empty,
            AnswerType = MapAnswerType(answer.AnswerType),
            AnswerValue = answer.AnswerValue
        };
    }

    public override async Task<AnswerResponse> UpdateAnswer(UpdateAnswerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid answer ID format"));
        }

        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Answer not found"));
        }

        answer.AnswerType = MapAnswerType(request.AnswerType);
        answer.AnswerValue = request.AnswerValue;
        await _answerRepository.UpdateAsync(answer);

        var user = await _userRepository.GetByIdAsync(answer.UserId);
        var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
        var template = await _templateRepository.GetByIdAsync(answer.TemplateId);

        return new AnswerResponse
        {
            Id = answer.Id.ToString(),
            UserId = answer.UserId.ToString(),
            UserName = user?.Name ?? string.Empty,
            QuestionId = answer.QuestionId.ToString(),
            QuestionTitle = question?.Title ?? string.Empty,
            TemplateId = answer.TemplateId.ToString(),
            TemplateTitle = template?.Title ?? string.Empty,
            AnswerType = MapAnswerType(answer.AnswerType),
            AnswerValue = answer.AnswerValue
        };
    }

    public override async Task<DeleteAnswerResponse> DeleteAnswer(DeleteAnswerRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid answer ID format"));
        }

        var answer = await _answerRepository.GetByIdAsync(id);
        if (answer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Answer not found"));
        }

        await _answerRepository.DeleteAsync(id);

        return new DeleteAnswerResponse
        {
            Success = true
        };
    }

    private static Protos.AnswerType MapAnswerType(Domain.Entities.AnswerType domainType)
    {
        return domainType switch
        {
            Domain.Entities.AnswerType.SingleLineString => Protos.AnswerType.SingleLineString,
            Domain.Entities.AnswerType.MultiLineText => Protos.AnswerType.MultiLineText,
            Domain.Entities.AnswerType.PositiveInteger => Protos.AnswerType.PositiveInteger,
            Domain.Entities.AnswerType.Checkbox => Protos.AnswerType.Checkbox,
            Domain.Entities.AnswerType.Boolean => Protos.AnswerType.Boolean,
            _ => throw new ArgumentException("Unknown answer type")
        };
    }

    private static Domain.Entities.AnswerType MapAnswerType(Protos.AnswerType protoType)
    {
        return protoType switch
        {
            Protos.AnswerType.SingleLineString => Domain.Entities.AnswerType.SingleLineString,
            Protos.AnswerType.MultiLineText => Domain.Entities.AnswerType.MultiLineText,
            Protos.AnswerType.PositiveInteger => Domain.Entities.AnswerType.PositiveInteger,
            Protos.AnswerType.Checkbox => Domain.Entities.AnswerType.Checkbox,
            Protos.AnswerType.Boolean => Domain.Entities.AnswerType.Boolean,
            _ => throw new ArgumentException("Unknown answer type")
        };
    }
}
