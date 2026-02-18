using Answer.Api.Protos;
using Answer.Application.Interfaces;
using Grpc.Core;

namespace Answer.Api.Services;

public class AnswerServiceImpl : AnswerService.AnswerServiceBase
{
    private readonly IRepository<Domain.Entities.Answer> _answerRepository;
    private readonly UserService.UserServiceClient _userServiceClient;
    private readonly QuestionService.QuestionServiceClient _questionServiceClient;
    private readonly TemplateService.TemplateServiceClient _templateServiceClient;

    public AnswerServiceImpl(
        IRepository<Domain.Entities.Answer> answerRepository,
        UserService.UserServiceClient userServiceClient,
        QuestionService.QuestionServiceClient questionServiceClient,
        TemplateService.TemplateServiceClient templateServiceClient)
    {
        _answerRepository = answerRepository;
        _userServiceClient = userServiceClient;
        _questionServiceClient = questionServiceClient;
        _templateServiceClient = templateServiceClient;
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

        // Call gRPC services to get related entity information
        var userName = string.Empty;
        var questionTitle = string.Empty;
        var templateTitle = string.Empty;

        try
        {
            var userResponse = await _userServiceClient.GetUserAsync(new GetUserRequest { Id = answer.UserId.ToString() });
            userName = userResponse.Name;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // User not found, leave empty
        }

        try
        {
            var questionResponse = await _questionServiceClient.GetQuestionAsync(new GetQuestionRequest { Id = answer.QuestionId.ToString() });
            questionTitle = questionResponse.Title;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Question not found, leave empty
        }

        try
        {
            var templateResponse = await _templateServiceClient.GetTemplateAsync(new GetTemplateRequest { Id = answer.TemplateId.ToString() });
            templateTitle = templateResponse.Title;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Template not found, leave empty
        }

        return new AnswerResponse
        {
            Id = answer.Id.ToString(),
            UserId = answer.UserId.ToString(),
            UserName = userName,
            QuestionId = answer.QuestionId.ToString(),
            QuestionTitle = questionTitle,
            TemplateId = answer.TemplateId.ToString(),
            TemplateTitle = templateTitle,
            AnswerType = MapAnswerType(answer.AnswerType),
            AnswerValue = answer.AnswerValue
        };
    }

    public override async Task<ListAnswersResponse> ListAnswers(ListAnswersRequest request, ServerCallContext context)
    {
        var answers = await _answerRepository.GetAllAsync();
        var response = new ListAnswersResponse();
        
        // Note: This makes multiple gRPC calls to retrieve related entity information.
        // For a production scenario with high performance requirements, consider implementing
        // caching, batch fetching, or returning only IDs and letting clients fetch related data.
        foreach (var answer in answers)
        {
            var userName = string.Empty;
            var questionTitle = string.Empty;
            var templateTitle = string.Empty;

            try
            {
                var userResponse = await _userServiceClient.GetUserAsync(new GetUserRequest { Id = answer.UserId.ToString() });
                userName = userResponse.Name;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                // User not found, leave empty
            }

            try
            {
                var questionResponse = await _questionServiceClient.GetQuestionAsync(new GetQuestionRequest { Id = answer.QuestionId.ToString() });
                questionTitle = questionResponse.Title;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                // Question not found, leave empty
            }

            try
            {
                var templateResponse = await _templateServiceClient.GetTemplateAsync(new GetTemplateRequest { Id = answer.TemplateId.ToString() });
                templateTitle = templateResponse.Title;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                // Template not found, leave empty
            }

            response.Answers.Add(new AnswerResponse
            {
                Id = answer.Id.ToString(),
                UserId = answer.UserId.ToString(),
                UserName = userName,
                QuestionId = answer.QuestionId.ToString(),
                QuestionTitle = questionTitle,
                TemplateId = answer.TemplateId.ToString(),
                TemplateTitle = templateTitle,
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

        // Call gRPC services to get related entity information
        var userName = string.Empty;
        var questionTitle = string.Empty;
        var templateTitle = string.Empty;

        try
        {
            var userResponse = await _userServiceClient.GetUserAsync(new GetUserRequest { Id = answer.UserId.ToString() });
            userName = userResponse.Name;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // User not found, leave empty
        }

        try
        {
            var questionResponse = await _questionServiceClient.GetQuestionAsync(new GetQuestionRequest { Id = answer.QuestionId.ToString() });
            questionTitle = questionResponse.Title;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Question not found, leave empty
        }

        try
        {
            var templateResponse = await _templateServiceClient.GetTemplateAsync(new GetTemplateRequest { Id = answer.TemplateId.ToString() });
            templateTitle = templateResponse.Title;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Template not found, leave empty
        }

        return new AnswerResponse
        {
            Id = answer.Id.ToString(),
            UserId = answer.UserId.ToString(),
            UserName = userName,
            QuestionId = answer.QuestionId.ToString(),
            QuestionTitle = questionTitle,
            TemplateId = answer.TemplateId.ToString(),
            TemplateTitle = templateTitle,
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

        // Call gRPC services to get related entity information
        var userName = string.Empty;
        var questionTitle = string.Empty;
        var templateTitle = string.Empty;

        try
        {
            var userResponse = await _userServiceClient.GetUserAsync(new GetUserRequest { Id = answer.UserId.ToString() });
            userName = userResponse.Name;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // User not found, leave empty
        }

        try
        {
            var questionResponse = await _questionServiceClient.GetQuestionAsync(new GetQuestionRequest { Id = answer.QuestionId.ToString() });
            questionTitle = questionResponse.Title;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Question not found, leave empty
        }

        try
        {
            var templateResponse = await _templateServiceClient.GetTemplateAsync(new GetTemplateRequest { Id = answer.TemplateId.ToString() });
            templateTitle = templateResponse.Title;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            // Template not found, leave empty
        }

        return new AnswerResponse
        {
            Id = answer.Id.ToString(),
            UserId = answer.UserId.ToString(),
            UserName = userName,
            QuestionId = answer.QuestionId.ToString(),
            QuestionTitle = questionTitle,
            TemplateId = answer.TemplateId.ToString(),
            TemplateTitle = templateTitle,
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
