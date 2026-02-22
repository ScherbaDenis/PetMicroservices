using Microsoft.Extensions.DependencyInjection;
using Template.Service.Services;

namespace Template.Service;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateServices(this IServiceCollection services)
    {
        services.AddScoped<ITemplateService, Services.TemplateService>();
        services.AddScoped<ITopicService, Services.TopicService>();
        services.AddScoped<IUserService, Services.UserService>();
        services.AddScoped<ITagService, Services.TagService>();
        services.AddScoped<IQuestionService, Services.QuestionService>();

        return services;
    }
}
