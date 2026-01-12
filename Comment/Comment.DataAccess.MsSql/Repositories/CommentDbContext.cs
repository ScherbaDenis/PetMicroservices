using Microsoft.EntityFrameworkCore;
using Comment.DataAccess.MsSql.EntityConfigurations;
using Comment.Domain.Models;

namespace Comment.DataAccess.MsSql.Repositories
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options)
        {
           
            //Database.EnsureCreated();
        }

        public DbSet<Domain.Models.Comment> Comments { get; set; }
        public DbSet<Template> Templates { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Comment");

            modelBuilder.ApplyConfiguration(new CommentEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new TemplateEntityConfigurtion());
        }
    }
}
