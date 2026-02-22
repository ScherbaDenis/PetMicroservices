using Comment.DataAccess.MsSql.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comment.DataAccess.MsSql.EntityConfigurations
{
    class CommentEntityConfiguration : IEntityTypeConfiguration<Domain.Models.Comment>
    {
        public void Configure(EntityTypeBuilder<Domain.Models.Comment> builder)
        {
            builder.ToTable("comments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.TemplateId);
            
            builder.HasIndex(x => x.TemplateId)
                .HasDatabaseName("IX_comments_TemplateId");

            builder.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
