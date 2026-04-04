using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.LearnerId)
                   .IsRequired();

            builder.HasIndex(u => u.LearnerId)
                   .IsUnique();

            builder.Property(u => u.Email)
                   .IsRequired();

            builder.Property(u => u.FullName)
                   .IsRequired();
        }
    }
}
