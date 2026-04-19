using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.CourseProgress.DTOs;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.CourseProgress.Repositories
{
    public interface ICourseProgressRepository
    {
        Task<decimal> CalculatePercentageAsync(Guid userId, Guid lessonId, CancellationToken ct);
        Task UpdateProgressAsync(Guid userId, Guid lessonId, decimal percentage, CancellationToken ct);
        Task<Guid?> GetNextLessonIdAsync(Guid currentLessonId, CancellationToken ct);
        Task<CourseProgressDto?> GetProgressAsync(Guid userId, Guid courseId, CancellationToken ct);

        // ✅ Explicitly return the entity
        Task<CourseProgressDto> GetByLearnerAndCourseAsync(Guid learnerId, Guid courseId, CancellationToken ct);
    }
}
