using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Mappings;
using TalentFlow.Application.LearnersProgress.Commands;
using TalentFlow.Application.Progresses.DTOs;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.LearnersProgress.Handlers
{
    public class UpdateLessonProgressHandler : IRequestHandler<UpdateLessonProgressCommand, ProgressDto>
    {
        private readonly IProgressRepository _progressRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLessonProgressHandler(IProgressRepository progressRepository, IUnitOfWork unitOfWork)
        {
            _progressRepository = progressRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProgressDto> Handle(UpdateLessonProgressCommand request, CancellationToken cancellationToken)
        {
            var progress = await _progressRepository.GetByLearnerAndLessonAsync(
                request.LearnerId, request.CourseId, request.LessonId, cancellationToken);

            if (progress == null)
            {
                progress = new LessonProgress(request.LearnerId, request.LessonId);
                await _progressRepository.AddAsync(progress, cancellationToken);
            }

            // ✅ Pass percentage + playback position
            progress.UpdateProgress((decimal)request.PercentageCompleted, request.VideoPosition);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return progress.ToDto(); // Extension method: LessonProgress → ProgressDto
        }
    }
}
