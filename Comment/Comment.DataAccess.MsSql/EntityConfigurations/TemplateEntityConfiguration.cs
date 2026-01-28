using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Comment.Domain.Models;

namespace Comment.DataAccess.MsSql.EntityConfigurations
{
    class TemplateEntityConfiguration : IEntityTypeConfiguration<Template>
    {
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            builder.ToTable("templates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.Title)
                .HasDatabaseName("IX_templates_Title");
        }
    }
}
