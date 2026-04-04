using TalentFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TalentFlow.Persistence
{
    public static class SeedData
    {
        public static async Task InitializeAsync(TalentFlowDbContext context)
        {
            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed Admin User
            if (!context.Users.Any())
            {
                var admin = new User("admin@talentflow.com", "System Admin", "admin-learner-id");
                context.Users.Add(admin);
            }

            /* Seed Sample Courses
            if (!context.Courses.Any())
            {
                var course1 = new Course("Introduction to TalentFlow", "Learn the basics of the TalentFlow platform.");
                var course2 = new Course("Advanced Event-Driven Architecture", "Deep dive into DDD, CQRS, and event streaming.");
                context.Courses.AddRange(course1, course2);
            }*/

            await context.SaveChangesAsync();
        }
    }
}
