using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Course>> GetCoursesByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Courses
                .Where(c => c.Enrollments.Any(e => e.UserId == userId && !e.IsDeleted))
                .Where(c => !c.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<Course?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.Slug == slug && !c.IsDeleted, ct);
        }

        public async Task<List<Course>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Courses
                .Where(c => !c.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<List<Course>> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct = default)
        {
            return await _context.Courses
                .Where(c => c.Enrollments.Any(e => e.UserId == learnerId && !e.IsDeleted) && !c.IsDeleted)
                .ToListAsync(ct);
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, ct);
        }

        public async Task AddAsync(Course course, CancellationToken ct = default)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            await _context.Courses.AddAsync(course, ct);
            
        }

        public async Task UpdateAsync(Course course, CancellationToken ct = default)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            _context.Courses.Update(course);
            await _context.SaveChangesAsync(ct);
        }

        public async Task SoftDeleteAsync(Course course, string deletedBy, CancellationToken ct = default)

        {
            if (course == null) throw new ArgumentNullException(nameof(course));

            course.SoftDelete(deletedBy);

            _context.Courses.Update(course);
            await _context.SaveChangesAsync(ct);
        }

    }
}
