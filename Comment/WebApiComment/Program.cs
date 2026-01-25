using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Comment.Domain.Repositories;
using Comment.Domain.Services;
using Comment.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//DbContext
builder.Services.AddControllers();// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CommentDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Services
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
