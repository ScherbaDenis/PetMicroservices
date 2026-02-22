using Comment.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Comment.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddCommentServices(this IServiceCollection services)
    {
        services.AddScoped<ITemplateService, Services.TemplateService>();
        services.AddScoped<ICommentService, Services.CommentService>();

        return services;
    }
}
