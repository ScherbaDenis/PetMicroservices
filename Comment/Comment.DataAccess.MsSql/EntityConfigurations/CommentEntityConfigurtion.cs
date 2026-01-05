using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comment.DataAccess.MsSql.EntityConfigurations
{
    class CommentEntityConfigurtion : IEntityTypeConfiguration<Domain.Models.Comment>
    {
        public void Configure(EntityTypeBuilder<Domain.Models.Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("comments", CommentDbContext.DEFAULT_SCHEMA);
            builder.HasIndex(x => x.Id);
            builder.HasIndex(x => x.Template);
        }
    }
}
