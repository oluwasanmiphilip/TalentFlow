using System;

namespace TalentFlow.Application.Lessons.DTOs
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public TimeSpan Duration { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ContentUrl { get; internal set; }
    }
}
