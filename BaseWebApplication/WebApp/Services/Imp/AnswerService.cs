using Answer.Api.Protos;
using Grpc.Net.Client;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class AnswerService : IAnswerService
    {
        private readonly GrpcChannel _channel;
        private readonly Answer.Api.Protos.AnswerService.AnswerServiceClient _client;

        public AnswerService(IConfiguration configuration)
        {
            var address = configuration["ApiEndpoints:AnswerService"]
                ?? throw new InvalidOperationException("AnswerService endpoint not configured.");

            _channel = GrpcChannel.ForAddress(address);
            _client = new Answer.Api.Protos.AnswerService.AnswerServiceClient(_channel);
        }

        public async Task<AnswerDto> CreateAsync(CreateAnswerDto answerDto, CancellationToken cancellationToken)
        {
            var request = new CreateAnswerRequest
            {
                UserId = answerDto.UserId.ToString(),
                QuestionId = answerDto.QuestionId.ToString(),
                TemplateId = answerDto.TemplateId.ToString(),
                AnswerType = MapAnswerType(answerDto.AnswerType),
                AnswerValue = answerDto.AnswerValue
            };

            var response = await _client.CreateAnswerAsync(request, cancellationToken: cancellationToken);

            return MapToDto(response);
        }

        public async Task DeleteAsync(Guid answerId, CancellationToken cancellationToken)
        {
            var request = new DeleteAnswerRequest
            {
                Id = answerId.ToString()
            };

            await _client.DeleteAnswerAsync(request, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<AnswerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var request = new ListAnswersRequest();
            var response = await _client.ListAnswersAsync(request, cancellationToken: cancellationToken);

            return response.Answers.Select(MapToDto);
        }

        public async Task<AnswerDto?> GetByIdAsync(Guid answerId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new GetAnswerRequest
                {
                    Id = answerId.ToString()
                };

                var response = await _client.GetAnswerAsync(request, cancellationToken: cancellationToken);
                return MapToDto(response);
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task UpdateAsync(UpdateAnswerDto answerDto, CancellationToken cancellationToken)
        {
            var request = new UpdateAnswerRequest
            {
                Id = answerDto.Id.ToString(),
                AnswerType = MapAnswerType(answerDto.AnswerType),
                AnswerValue = answerDto.AnswerValue
            };

            await _client.UpdateAnswerAsync(request, cancellationToken: cancellationToken);
        }

        private static AnswerDto MapToDto(AnswerResponse response)
        {
            return new AnswerDto
            {
                Id = Guid.Parse(response.Id),
                UserId = Guid.Parse(response.UserId),
                UserName = response.UserName,
                QuestionId = Guid.Parse(response.QuestionId),
                QuestionTitle = response.QuestionTitle,
                TemplateId = Guid.Parse(response.TemplateId),
                TemplateTitle = response.TemplateTitle,
                AnswerType = MapAnswerType(response.AnswerType),
                AnswerValue = response.AnswerValue
            };
        }

        private static Answer.Api.Protos.AnswerType MapAnswerType(DTOs.AnswerType dtoType)
        {
            return dtoType switch
            {
                DTOs.AnswerType.SingleLineString => Answer.Api.Protos.AnswerType.SingleLineString,
                DTOs.AnswerType.MultiLineText => Answer.Api.Protos.AnswerType.MultiLineText,
                DTOs.AnswerType.PositiveInteger => Answer.Api.Protos.AnswerType.PositiveInteger,
                DTOs.AnswerType.Checkbox => Answer.Api.Protos.AnswerType.Checkbox,
                DTOs.AnswerType.Boolean => Answer.Api.Protos.AnswerType.Boolean,
                _ => throw new ArgumentException("Unknown answer type")
            };
        }

        private static DTOs.AnswerType MapAnswerType(Answer.Api.Protos.AnswerType protoType)
        {
            return protoType switch
            {
                Answer.Api.Protos.AnswerType.SingleLineString => DTOs.AnswerType.SingleLineString,
                Answer.Api.Protos.AnswerType.MultiLineText => DTOs.AnswerType.MultiLineText,
                Answer.Api.Protos.AnswerType.PositiveInteger => DTOs.AnswerType.PositiveInteger,
                Answer.Api.Protos.AnswerType.Checkbox => DTOs.AnswerType.Checkbox,
                Answer.Api.Protos.AnswerType.Boolean => DTOs.AnswerType.Boolean,
                _ => throw new ArgumentException("Unknown answer type")
            };
        }
    }
}
