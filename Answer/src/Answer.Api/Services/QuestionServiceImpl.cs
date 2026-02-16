using Answer.Api.Protos;
using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using Grpc.Core;

namespace Answer.Api.Services;

public class QuestionServiceImpl : QuestionService.QuestionServiceBase
{
    private readonly IRepository<Question> _questionRepository;

    public QuestionServiceImpl(IRepository<Question> questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public override async Task<QuestionResponse> GetQuestion(GetQuestionRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid question ID format"));
        }

        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Question not found"));
        }

        return new QuestionResponse
        {
            Id = question.Id.ToString(),
            Title = question.Title
        };
    }

    public override async Task<ListQuestionsResponse> ListQuestions(ListQuestionsRequest request, ServerCallContext context)
    {
        var questions = await _questionRepository.GetAllAsync();
        var response = new ListQuestionsResponse();
        
        foreach (var question in questions)
        {
            response.Questions.Add(new QuestionResponse
            {
                Id = question.Id.ToString(),
                Title = question.Title
            });
        }

        return response;
    }

    public override async Task<QuestionResponse> CreateQuestion(CreateQuestionRequest request, ServerCallContext context)
    {
        var question = new Question
        {
            Title = request.Title
        };

        await _questionRepository.AddAsync(question);

        return new QuestionResponse
        {
            Id = question.Id.ToString(),
            Title = question.Title
        };
    }

    public override async Task<QuestionResponse> UpdateQuestion(UpdateQuestionRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid question ID format"));
        }

        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Question not found"));
        }

        question.Title = request.Title;
        await _questionRepository.UpdateAsync(question);

        return new QuestionResponse
        {
            Id = question.Id.ToString(),
            Title = question.Title
        };
    }

    public override async Task<DeleteQuestionResponse> DeleteQuestion(DeleteQuestionRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid question ID format"));
        }

        var question = await _questionRepository.GetByIdAsync(id);
        if (question == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Question not found"));
        }

        await _questionRepository.DeleteAsync(id);

        return new DeleteQuestionResponse
        {
            Success = true
        };
    }
}
