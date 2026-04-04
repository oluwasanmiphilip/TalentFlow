using Microsoft.EntityFrameworkCore;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence
{
    public class TalentFlowDbContext : DbContext
    {
        public TalentFlowDbContext(DbContextOptions<TalentFlowDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TalentFlowDbContext).Assembly);
        }
    }
}
