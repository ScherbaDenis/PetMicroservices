using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.EntityConfigurations
{
    class TopicEntityConfigurtion : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("topics", TemplateDbContext.DEFAULT_SCHEMA);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Name);
        }
    }
}
