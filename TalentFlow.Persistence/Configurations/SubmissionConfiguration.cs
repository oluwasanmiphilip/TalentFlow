using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Configurations
{
    public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ReferenceNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(s => s.Status)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(s => s.SubmittedBy)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(s => s.Text)
                   .HasMaxLength(5000);

            builder.Property(s => s.Url)
                   .HasMaxLength(2000);

            builder.Property(s => s.FilePath)
                   .HasMaxLength(500);

            builder.HasOne(s => s.Assessment)
                   .WithMany()
                   .HasForeignKey(s => s.AssignmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
