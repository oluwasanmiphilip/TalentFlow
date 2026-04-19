using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Enrollments.DTOs;
using TalentFlow.Application.Enrollments.Queries;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Enrollments.Handlers
{
    public class GetEnrollmentHandler : IRequestHandler<GetEnrollmentQuery, EnrollmentDto>
    {
        // ✅ ADD THIS LINE (MISSING BEFORE)
        private readonly IEnrollmentRepository _enrollmentRepository;

        // ✅ ADD CONSTRUCTOR
        public GetEnrollmentHandler(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<EnrollmentDto> Handle(GetEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository.GetByUserAndCourseAsync(
                request.UserId,
                request.CourseId,
                cancellationToken);

            if (enrollment == null)
                return null;

            return new EnrollmentDto
            {
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                EnrolledAt = enrollment.EnrolledAt,
                UpdatedAt = enrollment.UpdatedAt,
                UpdatedBy = enrollment.UpdatedBy,
                DeletedAt = enrollment.DeletedAt,
                DeletedBy = enrollment.DeletedBy,
                IsDeleted = enrollment.IsDeleted
            };
        }
    }
}