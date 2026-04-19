using System;
using MediatR;
using TalentFlow.Application.LeanersProgress.DTOs;

namespace TalentFlow.Application.LearnersProgress.Commands
{
    public class CompleteLessonCommand : IRequest<LeanerCompletionDto>
    {
        public Guid LearnerId { get; }
        public Guid CourseId { get; }
        public Guid LessonId { get; }

        public CompleteLessonCommand(Guid learnerId, Guid courseId, Guid lessonId)
        {
            LearnerId = learnerId;
            CourseId = courseId;
            LessonId = lessonId;
        }
    }
}
