using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.Configurations
{
    public class QuestionEntityConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("questions");

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(q => q.Title)
                .HasDatabaseName("IX_questions_title");

            builder.Property(q => q.Description)
                .HasMaxLength(2000);

            // Configure Table Per Hierarchy (TPH) inheritance
            builder.HasDiscriminator<string>("QuestionType")
                .HasValue<SingleLineStringQuestion>("SingleLineString")
                .HasValue<MultiLineTextQuestion>("MultiLineText")
                .HasValue<PositiveIntegerQuestion>("PositiveInteger")
                .HasValue<CheckboxQuestion>("Checkbox")
                .HasValue<BooleanQuestion>("Boolean");

            // If Question is related to Template via TemplateId
            builder.Property<Guid?>("TemplateId");
            builder.HasIndex("TemplateId");

            builder.HasOne<Domain.Model.Template>()
                .WithMany(t => t.Questions)
                .HasForeignKey("TemplateId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
