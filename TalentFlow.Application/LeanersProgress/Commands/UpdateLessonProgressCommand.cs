using System;
using MediatR;
using TalentFlow.Application.Progresses.DTOs;

namespace TalentFlow.Application.LearnersProgress.Commands
{
    public class UpdateLessonProgressCommand : IRequest<ProgressDto>
    {
        public Guid LearnerId { get; }
        public Guid CourseId { get; }
        public Guid LessonId { get; }
        public double PercentageCompleted { get; }
        public TimeSpan? VideoPosition { get; }

        public UpdateLessonProgressCommand(Guid learnerId, Guid courseId, Guid lessonId, double percentageCompleted, TimeSpan? videoPosition)
        {
            LearnerId = learnerId;
            CourseId = courseId;
            LessonId = lessonId;
            PercentageCompleted = percentageCompleted;
            VideoPosition = videoPosition;
        }
    }
}
