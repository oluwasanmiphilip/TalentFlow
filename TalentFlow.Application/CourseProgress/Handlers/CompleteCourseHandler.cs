using System;
using MediatR;
using TalentFlow.Application.CourseProgress.DTOs;

namespace TalentFlow.Application.CourseProgress.Commands
{
    // ✅ Must implement IRequest<CourseProgressDto>
    public class CompleteCourseCommand : IRequest<CourseProgressDto>
    {
        public Guid LearnerId { get; }
        public Guid CourseId { get; }

        public CompleteCourseCommand(Guid learnerId, Guid courseId)
        {
            LearnerId = learnerId;
            CourseId = courseId;
        }
    }
}
