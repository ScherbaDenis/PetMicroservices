using Answer.Api.Services;
using Answer.Infrastructure;
using Answer.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();
builder.Services.AddControllers();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

// Map REST API endpoints that return direct arrays
// These will override the gRPC-JSON transcoding endpoints for List operations
app.MapGet("/api/answers", async (Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.Answer> answerRepo,
                                    Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.User> userRepo,
                                    Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.Question> questionRepo,
                                    Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.Template> templateRepo) =>
{
    var answers = await answerRepo.GetAllAsync();
    var result = new List<object>();
    foreach (var answer in answers)
    {
        var user = await userRepo.GetByIdAsync(answer.UserId);
        var question = await questionRepo.GetByIdAsync(answer.QuestionId);
        var template = await templateRepo.GetByIdAsync(answer.TemplateId);
        
        result.Add(new
        {
            id = answer.Id.ToString(),
            userId = answer.UserId.ToString(),
            userName = user?.Name ?? string.Empty,
            questionId = answer.QuestionId.ToString(),
            questionTitle = question?.Title ?? string.Empty,
            templateId = answer.TemplateId.ToString(),
            templateTitle = template?.Title ?? string.Empty,
            answerType = answer.AnswerType.ToString(),
            answerValue = answer.AnswerValue
        });
    }
    return Results.Ok(result);
});

app.MapGet("/api/users", async (Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.User> userRepo) =>
{
    var users = await userRepo.GetAllAsync();
    var result = users.Select(u => new { id = u.Id.ToString(), name = u.Name });
    return Results.Ok(result);
});

app.MapGet("/api/questions", async (Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.Question> questionRepo) =>
{
    var questions = await questionRepo.GetAllAsync();
    var result = questions.Select(q => new { id = q.Id.ToString(), title = q.Title });
    return Results.Ok(result);
});

app.MapGet("/api/templates", async (Answer.Application.Interfaces.IRepository<Answer.Domain.Entities.Template> templateRepo) =>
{
    var templates = await templateRepo.GetAllAsync();
    var result = templates.Select(t => new { id = t.Id.ToString(), title = t.Title });
    return Results.Ok(result);
});

// Map gRPC services
app.MapGrpcService<UserServiceImpl>().EnableGrpcWeb();
app.MapGrpcService<QuestionServiceImpl>().EnableGrpcWeb();
app.MapGrpcService<TemplateServiceImpl>().EnableGrpcWeb();
app.MapGrpcService<AnswerServiceImpl>().EnableGrpcWeb();

// Enable gRPC reflection for development
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () => "Answer API - gRPC with REST support");

app.Run();

// Make the Program class accessible to integration tests
public partial class Program { }
