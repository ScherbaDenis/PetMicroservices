using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.DataAccess.MsSql.Repositories;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.EntityConfigurations
{
    class TamplateEntityConfigurtion : IEntityTypeConfiguration<Tamplate>
    {
        public void Configure(EntityTypeBuilder<Tamplate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("users", TamplateDbContext.DEFAULT_SCHEMA);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Tags);
            builder.HasIndex(x => x.Title);
        }
    }
}
