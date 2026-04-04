using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly TalentFlowDbContext _context;

        public CourseRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
            await _context.Courses.Include(c => c.Enrollments)
                                  .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);

        public async Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await _context.Courses.Include(c => c.Enrollments).ToListAsync(cancellationToken);

        public async Task<List<Course>> GetByLearnerIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
            await _context.Courses
                .Include(c => c.Enrollments)
                .Where(c => c.Enrollments.Any(e => e.UserId == userId))
                .ToListAsync(cancellationToken);

        public async Task AddAsync(Course course, CancellationToken cancellationToken = default) =>
            await _context.Courses.AddAsync(course, cancellationToken);

        public async Task UpdateAsync(Course course, CancellationToken cancellationToken = default)
        {
            _context.Courses.Update(course);
            await Task.CompletedTask;
        }
        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Courses.FindAsync(new object[] { id }, cancellationToken);
        }

    }
}
