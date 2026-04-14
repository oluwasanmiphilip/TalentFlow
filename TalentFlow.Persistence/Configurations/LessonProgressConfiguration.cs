using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Configurations
{
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            builder.HasKey(lp => lp.Id);

            builder.HasIndex(lp => new { lp.LessonId, lp.UserId })
                   .IsUnique();

            builder.Property(lp => lp.VideoPositionSeconds)
                   .HasDefaultValue(0);

            builder.Property(lp => lp.CoursePercentage)
                   .HasDefaultValue(0);

            builder.HasOne(lp => lp.Lesson)
                   .WithMany()
                   .HasForeignKey(lp => lp.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lp => lp.User)
                   .WithMany()
                   .HasForeignKey(lp => lp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
