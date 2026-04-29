using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Courses.Queries;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Courses.Handlers
{
    public class GetCourseEnrollmentHandler
        : IRequestHandler<GetCourseEnrollmentQuery, List<EnrollmentDto>>
    {
        private readonly IEnrollmentRepository _repo;

        public GetCourseEnrollmentHandler(IEnrollmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<EnrollmentDto>> Handle(GetCourseEnrollmentQuery request, CancellationToken ct)
        {
            var enrollments = await _repo.GetByCourseIdAsync(request.CourseId, ct);
            if (enrollments == null || !enrollments.Any()) return new List<EnrollmentDto>();

            return enrollments.Select(e => new EnrollmentDto
            {
                Id = e.Id,
                CourseId = e.CourseId,
                UserId = e.UserId,
                EnrolledAt = e.EnrolledAt,
                IsDeleted = e.IsDeleted,
                DeletedBy = e.DeletedBy,
                DeletedAt = e.DeletedAt
            }).ToList();
        }
    }
}
