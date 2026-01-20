using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.EntityConfigurations
{
    class TemplateEntityConfigurtion : IEntityTypeConfiguration<Domain.Model.Template>
    {
        public void Configure(EntityTypeBuilder<Domain.Model.Template> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("templates");
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Title);
        }
    }
}
