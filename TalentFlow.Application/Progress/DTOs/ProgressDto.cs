using System;

namespace TalentFlow.Application.Progresses.DTOs
{
    public class ProgressDto
    {
        public Guid Id { get; set; }
        public Guid LearnerId { get; set; }
        public Guid CourseId { get; set; }
        public Guid LessonId { get; set; }

        // ✅ Track video position
        public int VideoPositionSeconds { get; set; }

        // ✅ Completion timestamp
        public DateTime? CompletedAt { get; set; }

        // ✅ Course-level progress
        public decimal CoursePercentage { get; set; }

        public DateTime LastAccessed { get; set; }
    }
}
