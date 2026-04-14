using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Enrollments.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Enrollments.Handlers
{
    public class EnrollLearnerHandler : IRequestHandler<EnrollLearnerCommand, bool>
    {
        private readonly ICourseRepository _courseRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;

        public EnrollLearnerHandler(ICourseRepository courseRepo, IEnrollmentRepository enrollmentRepo)
        {
            _courseRepo = courseRepo ?? throw new ArgumentNullException(nameof(courseRepo));
            _enrollmentRepo = enrollmentRepo ?? throw new ArgumentNullException(nameof(enrollmentRepo));
        }

        public async Task<bool> Handle(EnrollLearnerCommand request, CancellationToken ct)
        {
            var course = await _courseRepo.GetByIdAsync(request.CourseId, ct);
            if (course == null || course.IsDeleted) return false;

            var enrollment = new Enrollment(request.CourseId, request.UserId, "Learner");


            enrollment.Update(request.EnrolledBy);

            await _enrollmentRepo.AddAsync(enrollment, ct);
            return true;
        }
    }
}
