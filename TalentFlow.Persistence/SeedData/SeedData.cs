using Microsoft.EntityFrameworkCore;
using TalentFlow.Domain.Entities;
using BCrypt.Net;

namespace TalentFlow.Persistence
{
    public static class SeedData
    {
        public static async Task InitializeAsync(TalentFlowDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!context.Users.Any())
            {
                // Admin seed
                var admin = new User(
                    "admin@talentflow.com",                       // email
                    "System Admin",                               // full name
                    BCrypt.Net.BCrypt.HashPassword("Admin123!"),  // password hash
                    "Admin",                                      // role
                    "System Administration",                      // discipline
                    DateTime.UtcNow.Year,                         // cohort year
                    "+2348000000000"                              // phone number
                );

                // Learner seed
                var learner = new User(
                    "learner@talentflow.com",                     // email
                    "Default Learner",                            // full name
                    BCrypt.Net.BCrypt.HashPassword("Learner123!"),// password hash
                    "Learner",                                    // role
                    "General Studies",                            // discipline
                    DateTime.UtcNow.Year,                         // cohort year
                    "+2348111111111"                              // phone number
                );

                context.Users.AddRange(admin, learner);
            }

            await context.SaveChangesAsync();
        }
    }
}
