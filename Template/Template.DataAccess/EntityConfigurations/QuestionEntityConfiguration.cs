using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.Configurations
{
    public class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Id)
                .IsRequired();

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(q => q.Description)
                .HasMaxLength(2000);
        }
    }
}
