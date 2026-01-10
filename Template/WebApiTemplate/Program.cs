using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Repository;
using Template.Domain.Services;
using Template.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
