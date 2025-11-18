using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Comment.Domain.Models;

namespace Comment.DataAccess.MsSql.EntityConfigurations
{
    class TamplateEntityConfigurtion : IEntityTypeConfiguration<Tamplate>
    {
        public void Configure(EntityTypeBuilder<Tamplate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("tamplates", CommentDbContext.DEFAULT_SCHEMA);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Title);
        }
    }
}
