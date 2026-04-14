using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.LearningProgress.Repositories
{
    public interface ILessonProgressRepository
    {
        object CompletedAt { get; }
        int VideoPositionSeconds { get; set; }

        Task<ILessonProgressRepository?> GetAsync(Guid lessonId, Guid userId, CancellationToken ct);
        Task AddAsync(ILessonProgressRepository progress, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}