using System;
using MediatR;
using TalentFlow.Application.Lessons.DTOs;

namespace TalentFlow.Application.Lessons.Commands
{
    public class CreateLessonCommand : IRequest<LessonDto>
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty; // ✅ Added
        public int Order { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
