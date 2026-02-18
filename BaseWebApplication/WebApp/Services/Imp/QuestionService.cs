using Answer.Api.Protos;
using Grpc.Net.Client;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class QuestionService : IQuestionService
    {
        private readonly GrpcChannel _channel;
        private readonly Answer.Api.Protos.QuestionService.QuestionServiceClient _client;

        public QuestionService(IConfiguration configuration)
        {
            var address = configuration["ApiEndpoints:QuestionService"]
                ?? configuration["ApiEndpoints:AnswerService"]
                ?? throw new InvalidOperationException("QuestionService or AnswerService endpoint not configured.");

            _channel = GrpcChannel.ForAddress(address);
            _client = new Answer.Api.Protos.QuestionService.QuestionServiceClient(_channel);
        }

        public async Task<QuestionDto> CreateAsync(QuestionDto questionDto, CancellationToken cancellationToken)
        {
            var request = new CreateQuestionRequest
            {
                Title = questionDto.Title
            };

            var response = await _client.CreateQuestionAsync(request, cancellationToken: cancellationToken);

            return MapToDto(response);
        }

        public async Task DeleteAsync(Guid questionId, CancellationToken cancellationToken)
        {
            var request = new DeleteQuestionRequest
            {
                Id = questionId.ToString()
            };

            await _client.DeleteQuestionAsync(request, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<QuestionDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var request = new ListQuestionsRequest();
            var response = await _client.ListQuestionsAsync(request, cancellationToken: cancellationToken);

            return response.Questions.Select(MapToDto);
        }

        public async Task<QuestionDto?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetQuestionRequest
                {
                    Id = questionId.ToString()
                };

                var response = await _client.GetQuestionAsync(request, cancellationToken: cancellationToken);
                return MapToDto(response);
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task UpdateAsync(QuestionDto questionDto, CancellationToken cancellationToken)
        {
            var request = new UpdateQuestionRequest
            {
                Id = questionDto.Id.ToString(),
                Title = questionDto.Title
            };

            await _client.UpdateQuestionAsync(request, cancellationToken: cancellationToken);
        }

        private static QuestionDto MapToDto(QuestionResponse response)
        {
            // Default to SingleLineString type if not specified
            return new SingleLineStringQuestionDto
            {
                Id = Guid.Parse(response.Id),
                Title = response.Title
            };
        }
    }
}
