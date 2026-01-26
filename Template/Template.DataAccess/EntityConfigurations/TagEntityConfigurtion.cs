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
            builder.HasKey(x => x.Id);
            builder.ToTable("tags");
            builder.HasIndex(x => x.Name);
        }
    }
}
