using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Configurations
{
    public class CourseProgressConfiguration : IEntityTypeConfiguration<CourseProgress>
    {
        public void Configure(EntityTypeBuilder<CourseProgress> builder)
        {
            builder.HasKey(cp => cp.Id);

            builder.HasIndex(cp => new { cp.CourseId, cp.UserId })
                   .IsUnique();

            builder.Property(cp => cp.Percentage)
                   .HasDefaultValue(0);

            builder.Property(cp => cp.CertificateUnlocked)
                   .HasDefaultValue(false);
        }
    }
}
