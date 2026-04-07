// File Path: TalentFlow.Persistence/TalentFlowDbContext.cs

using Microsoft.EntityFrameworkCore;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence
{
    public class TalentFlowDbContext : DbContext
    {
        public TalentFlowDbContext(DbContextOptions<TalentFlowDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Instructor> Instructors { get; set; } = null!;
        public DbSet<Assessment> Assessments { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Progress> Progresses { get; set; } = null!;
        public DbSet<Lesson> Lessons { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Certificate> Certificates { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;

        // 👇 RefreshToken DbSet
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit configuration for User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.LearnerId).IsRequired().HasColumnType("uniqueidentifier");
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
            });

            // ✅ Configure RefreshToken entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);
                entity.Property(rt => rt.Token).IsRequired();
                entity.Property(rt => rt.Email).IsRequired().HasMaxLength(255);
                entity.Property(rt => rt.Role).IsRequired().HasMaxLength(50);
                entity.Property(rt => rt.ExpiresAt).IsRequired();
                entity.Property(rt => rt.IsRevoked).IsRequired();
            });

            modelBuilder.Entity<Assessment>().Ignore(a => a.Questions);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TalentFlowDbContext).Assembly);
        }
    }
}
