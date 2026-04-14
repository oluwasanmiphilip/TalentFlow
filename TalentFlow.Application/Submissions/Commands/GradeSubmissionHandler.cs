using MediatR;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Submissions.Commands
{
    public class GradeSubmissionHandler : IRequestHandler<GradeSubmissionCommand, bool>
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GradeSubmissionHandler(
            ISubmissionRepository submissionRepository,
            IUnitOfWork unitOfWork)
        {
            _submissionRepository = submissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
        {
            var submission = await _submissionRepository
                .GetByIdAsync(request.SubmissionId, cancellationToken);

            if (submission == null)
                return false;

            submission.Score = request.Score;
            submission.InstructorComment = request.InstructorComment;
            submission.Status = "graded"; // replace with enum later

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}