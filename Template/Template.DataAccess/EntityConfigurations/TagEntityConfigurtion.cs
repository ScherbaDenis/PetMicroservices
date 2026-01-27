using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.EntityConfigurations
{
    class TagEntityConfigurtion : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_tags_name");
        }
    }
}
