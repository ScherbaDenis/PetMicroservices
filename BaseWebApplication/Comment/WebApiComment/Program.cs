using Comment.DataAccess.MsSql.Repositories;
using Comment.Domain.Repositories;
using Comment.Domain.Services;
using Comment.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ITamplateRepository, TamplateRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();


builder.Services.AddScoped<ITamplateService, TamplateService>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
