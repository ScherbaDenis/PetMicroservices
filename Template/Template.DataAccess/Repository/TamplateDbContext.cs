using Microsoft.EntityFrameworkCore;
using Template.DataAccess.MsSql.EntityConfigurations;
using Template.Domain.Model;
using Template.Domain.Repository;

namespace Template.DataAccess.MsSql.Repository
{
    public class TamplateDbContext : DbContext, IUnitOfWork
    {
        public TamplateDbContext(DbContextOptions<TamplateDbContext> options) : base(options) { }

        public const string DEFAULT_SCHEMA = "ordering";

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public DbSet<Tamplate> Tamplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TagEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new UserEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new TopicEntityConfigurtion());
            modelBuilder.ApplyConfiguration(new TamplateEntityConfigurtion());
        }

    }
}
