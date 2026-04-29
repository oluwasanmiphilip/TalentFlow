using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.HasMany(c => c.Enrollments)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            builder.Property(c => c.ThumbnailUrl).HasMaxLength(500);
            builder.Property(c => c.Level).HasMaxLength(50);
            builder.Property(c => c.Price).HasColumnType("decimal(18,2)");
            builder.Property(c => c.DurationMinutes);
            builder.Property(c => c.Rating).HasColumnType("float");
            builder.Property(c => c.Tags)
                   .HasConversion(
                       v => string.Join(',', v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                   );

        }
    }
}
