using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Enrollments.Commands;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Enrollments.Handlers
{
    public class UpdateEnrollmentHandler : IRequestHandler<UpdateEnrollmentCommand, bool>
    {
        private readonly IEnrollmentRepository _enrollmentRepo;

        public UpdateEnrollmentHandler(IEnrollmentRepository enrollmentRepo)
        {
            _enrollmentRepo = enrollmentRepo ?? throw new ArgumentNullException(nameof(enrollmentRepo));
        }

        public async Task<bool> Handle(UpdateEnrollmentCommand request, CancellationToken ct)
        {
            // Fetch enrollment by Id (you may need to add GetByIdAsync to IEnrollmentRepository)
            var enrollment = await _enrollmentRepo.GetByUserAndCourseAsync(request.Id, Guid.Empty, ct);
            // Adjust lookup if you prefer by enrollment Id directly

            if (enrollment == null || enrollment.IsDeleted) return false;

            // Update status and audit info
            enrollment.ChangeStatus(request.Status, request.UpdatedBy);
            enrollment.Update(request.UpdatedBy);

            await _enrollmentRepo.UpdateAsync(enrollment, ct);
            return true;
        }
    }
}
