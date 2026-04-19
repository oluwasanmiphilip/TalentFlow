using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.LearnersProgress.Commands;
using TalentFlow.Application.LeanersProgress.DTOs;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.LearnersProgress.Handlers
{
    public class LessonProgressHandler :
        IRequestHandler<CompleteLessonCommand, LeanerCompletionDto>
    {
        private readonly IProgressRepository _progressRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LessonProgressHandler(
            IProgressRepository progressRepository,
            ILessonRepository lessonRepository,
            IUnitOfWork unitOfWork)
        {
            _progressRepository = progressRepository;
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<LeanerCompletionDto> Handle(
            CompleteLessonCommand request,
            CancellationToken cancellationToken)
        {
            var progress = await _progressRepository.GetByLearnerAndLessonAsync(
                request.LearnerId, request.CourseId, request.LessonId, cancellationToken);

            if (progress == null)
            {
                progress = new LessonProgress(request.LearnerId, request.LessonId);
                await _progressRepository.AddAsync(progress, cancellationToken);
            }

            progress.MarkComplete();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var nextLesson = await _lessonRepository.GetNextLessonAsync(
                request.CourseId, request.LessonId, cancellationToken);

            return new LeanerCompletionDto
            {
                CompletedAt = progress.CompletedAt ?? DateTime.UtcNow,
                NextLessonId = nextLesson?.Id,
                CoursePercentage = progress.CoursePercentage
            };
        }
    }
}
