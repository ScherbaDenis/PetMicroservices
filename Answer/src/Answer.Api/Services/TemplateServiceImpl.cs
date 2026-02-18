using Answer.Api.Protos;
using Answer.Application.Interfaces;
using Answer.Domain.Entities;
using Grpc.Core;

namespace Answer.Api.Services;

public class TemplateServiceImpl : TemplateService.TemplateServiceBase
{
    private readonly IRepository<Template> _templateRepository;

    public TemplateServiceImpl(IRepository<Template> templateRepository)
    {
        _templateRepository = templateRepository;
    }

    public override async Task<TemplateResponse> GetTemplate(GetTemplateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid template ID format"));
        }

        var template = await _templateRepository.GetByIdAsync(id);
        if (template == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Template not found"));
        }

        return new TemplateResponse
        {
            Id = template.Id.ToString(),
            Title = template.Title
        };
    }

    public override async Task ListTemplates(ListTemplatesRequest request, IServerStreamWriter<TemplateResponse> responseStream, ServerCallContext context)
    {
        var templates = await _templateRepository.GetAllAsync();
        
        foreach (var template in templates)
        {
            await responseStream.WriteAsync(new TemplateResponse
            {
                Id = template.Id.ToString(),
                Title = template.Title
            });
        }
    }

    public override async Task<TemplateResponse> CreateTemplate(CreateTemplateRequest request, ServerCallContext context)
    {
        var template = new Template
        {
            Title = request.Title
        };

        await _templateRepository.AddAsync(template);

        return new TemplateResponse
        {
            Id = template.Id.ToString(),
            Title = template.Title
        };
    }

    public override async Task<TemplateResponse> UpdateTemplate(UpdateTemplateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid template ID format"));
        }

        var template = await _templateRepository.GetByIdAsync(id);
        if (template == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Template not found"));
        }

        template.Title = request.Title;
        await _templateRepository.UpdateAsync(template);

        return new TemplateResponse
        {
            Id = template.Id.ToString(),
            Title = template.Title
        };
    }

    public override async Task<DeleteTemplateResponse> DeleteTemplate(DeleteTemplateRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid template ID format"));
        }

        var template = await _templateRepository.GetByIdAsync(id);
        if (template == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Template not found"));
        }

        await _templateRepository.DeleteAsync(id);

        return new DeleteTemplateResponse
        {
            Success = true
        };
    }
}
