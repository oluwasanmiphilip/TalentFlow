using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly TalentFlowDbContext _context;

        public ProgressRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task<LessonProgress?> GetByLearnerAndLessonAsync(Guid learnerId, Guid courseId, Guid lessonId, CancellationToken cancellationToken)
        {
            return await _context.LessonProgresses
                .Include(lp => lp.Lesson)
                .FirstOrDefaultAsync(lp =>
                    lp.UserId == learnerId &&
                    lp.LessonId == lessonId &&
                    lp.Lesson.CourseId == courseId,
                    cancellationToken);
        }

        public async Task AddAsync(LessonProgress progress, CancellationToken cancellationToken)
        {
            await _context.LessonProgresses.AddAsync(progress, cancellationToken);
        }

        public async Task UpdateAsync(LessonProgress progress, CancellationToken cancellationToken)
        {
            _context.LessonProgresses.Update(progress);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var progress = await _context.LessonProgresses.FindAsync(new object[] { id }, cancellationToken);
            if (progress != null)
            {
                _context.LessonProgresses.Remove(progress);
            }
        }
    }
}
