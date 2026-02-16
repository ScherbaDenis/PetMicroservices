using Answer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Answer.Infrastructure.Data;

public class AnswerDbContext : DbContext
{
    public AnswerDbContext(DbContextOptions<AnswerDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Question> Questions { get; set; } = null!;
    public DbSet<Template> Templates { get; set; } = null!;
    public DbSet<Domain.Entities.Answer> Answers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
        });

        // Configure Question entity
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);
        });

        // Configure Template entity
        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
        });

        // Configure Answer entity
        modelBuilder.Entity<Domain.Entities.Answer>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.AnswerValue)
                .IsRequired()
                .HasMaxLength(4000);

            entity.Property(e => e.AnswerType)
                .IsRequired();

            // Configure relationships
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Question)
                .WithMany()
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Template)
                .WithMany()
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
