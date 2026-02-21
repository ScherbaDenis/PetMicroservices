using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Domain.Model;

namespace Template.DataAccess.MsSql.EntityConfigurations
{
    class TemplateEntityConfiguration : IEntityTypeConfiguration<Domain.Model.Template>
    {
        public void Configure(EntityTypeBuilder<Domain.Model.Template> builder)
        {
            builder.ToTable("templates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.Title);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            // Owner (many templates to one user)
            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Topic (many templates to one topic)
            builder.HasOne(x => x.Topic)
                .WithMany()
                .HasForeignKey(x => x.TopicId)
                .OnDelete(DeleteBehavior.SetNull);

            // Tags (many-to-many)
            builder.HasMany(x => x.Tags)
                .WithMany();

            // UsersAccess (many-to-many)
            builder.HasMany(x => x.UsersAccess)
                .WithMany();

            // Questions (one-to-many)
            builder.HasMany(x => x.Questions)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
