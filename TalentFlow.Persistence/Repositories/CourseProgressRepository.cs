using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.CourseProgress.Repositories;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class CourseProgressRepository 
    {
        private readonly TalentFlowDbContext _db;

        public CourseProgressRepository(TalentFlowDbContext db) => _db = db;

        public async Task<decimal> CalculatePercentageAsync(Guid userId, Guid lessonId, CancellationToken ct)
        {
            var lesson = await _db.Lessons.FindAsync(new object[] { lessonId }, ct);
            if (lesson == null) return 0;

            var courseId = lesson.CourseId;
            var totalLessons = await _db.Lessons.CountAsync(l => l.CourseId == courseId, ct);
            if (totalLessons == 0) return 0;

            var completedLessons = await _db.LessonProgresses
                .CountAsync(lp => lp.UserId == userId && lp.CompletedAt != null &&
                                  _db.Lessons.Any(l => l.Id == lp.LessonId && l.CourseId == courseId), ct);

            return Math.Round((decimal)completedLessons / totalLessons * 100, 2);
        }

        public async Task UpdateProgressAsync(Guid userId, Guid lessonId, decimal percentage, CancellationToken ct)
        {
            var lesson = await _db.Lessons.FindAsync(new object[] { lessonId }, ct);
            if (lesson == null) return;

            var courseId = lesson.CourseId;
            var courseProgress = await _db.CourseProgresses
                .FirstOrDefaultAsync(cp => cp.CourseId == courseId && cp.UserId == userId, ct);

            if (courseProgress == null)
            {
                courseProgress = new CourseProgress
                {
                    Id = Guid.NewGuid(),
                    CourseId = courseId,
                    UserId = userId,
                    Percentage = percentage,
                    CertificateUnlocked = percentage >= 100,
                    CompletedAt = percentage >= 100 ? DateTime.UtcNow : null
                };
                await _db.CourseProgresses.AddAsync(courseProgress, ct);
            }
            else
            {
                courseProgress.Percentage = percentage;
                courseProgress.CertificateUnlocked = percentage >= 100;
                courseProgress.CompletedAt = percentage >= 100 ? DateTime.UtcNow : null;
            }
        }

        public async Task<Guid?> GetNextLessonIdAsync(Guid currentLessonId, CancellationToken ct)
        {
            var currentLesson = await _db.Lessons.FindAsync(new object[] { currentLessonId }, ct);
            if (currentLesson == null) return null;

            return await _db.Lessons
                .Where(l => l.CourseId == currentLesson.CourseId && l.Order > currentLesson.Order)
                .OrderBy(l => l.Order)
                .Select(l => l.Id)
                .FirstOrDefaultAsync(ct);
        }
    }
}
