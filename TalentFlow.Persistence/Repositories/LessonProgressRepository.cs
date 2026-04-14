using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.LessonProgress;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class LessonProgressRepository 
    {
        private readonly TalentFlowDbContext _db;

        public LessonProgressRepository(TalentFlowDbContext db)
        {
            _db = db;
        }

        public async Task<LessonProgress?> GetAsync(Guid lessonId, Guid userId, CancellationToken ct)
        {
            return await _db.LessonProgresses
                .FirstOrDefaultAsync(lp => lp.LessonId == lessonId && lp.UserId == userId, ct);
        }

        public async Task AddAsync(LessonProgress progress, CancellationToken ct)
        {
            await _db.LessonProgresses.AddAsync(progress, ct);
        }
    }
}
