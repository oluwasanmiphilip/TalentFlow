// File Path: TalentFlow.Persistence/TalentFlowDbContext.cs

using Microsoft.EntityFrameworkCore;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence
{
    public class TalentFlowDbContext : DbContext
    {
        private readonly DomainEventDispatcher? _dispatcher;

        public TalentFlowDbContext(DbContextOptions<TalentFlowDbContext> options, DomainEventDispatcher dispatcher)
            : base(options)
        {
            _dispatcher = dispatcher;
        }

        // ✅ Used at design-time (migrations)
        public TalentFlowDbContext(DbContextOptions<TalentFlowDbContext> options)
            : base(options)
        {
            // No dispatcher needed for tooling
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
        public DbSet<CourseProgress> CourseProgresses { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<Certificate> Certificates { get; set; } = null!;
        public DbSet<Video> Videos { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; } = null!;
        public DbSet<Submission> Submissions { get; set; } = null!;
        public DbSet<LearningWork> LearningWorks { get; set; } = null!;

        public DbSet<LessonProgress> LessonProgresses { get; set; } = null!;
        public DbSet<ProfileUser> ProfileUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit configuration for User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.LearnerId).IsRequired();
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

            // ✅ Configure OTP table
            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.ToTable("otp_codes");
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Code).IsRequired().HasMaxLength(6);
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.ProfileUser)
                .WithOne(p => p.User)
                .HasForeignKey<ProfileUser>(p => p.UserId);


            // Ignore navigation property in Assessment
            modelBuilder.Entity<Assessment>().Ignore(a => a.Questions);

            // ✅ Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role(new Guid("11111111-1111-1111-1111-111111111111"), "Admin"),
                new Role(new Guid("22222222-2222-2222-2222-222222222222"), "Instructor"),
                new Role(new Guid("33333333-3333-3333-3333-333333333333"), "Learner")
            );

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TalentFlowDbContext).Assembly);
        }

        // 👇 Domain Event Dispatching
        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            DispatchDomainEvents();
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            DispatchDomainEvents();
            return result;
        }

        private void DispatchDomainEvents()
        {
            if (_dispatcher == null) return;

            var entitiesWithEvents = ChangeTracker
                .Entries<EntityBase>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (var domainEvent in events)
                {
                    _dispatcher.Dispatch(domainEvent);
                }
            }
        }
    }
}
