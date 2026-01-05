using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Comment.Domain.Models;

namespace Comment.DataAccess.MsSql.EntityConfigurations
{
    class TemplateEntityConfigurtion : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("templates", CommentDbContext.DEFAULT_SCHEMA);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Title);
        }
    }
}
