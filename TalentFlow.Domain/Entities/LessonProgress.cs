using System;

namespace TalentFlow.Domain.Entities
{
    public class LessonProgress
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; } = Guid.Empty;
        public Guid UserId { get; set; }

        public int VideoPositionSeconds { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Progress tracking
        public decimal CoursePercentage { get; set; }

        // Navigation
        public Lesson? Lesson { get; set; }
        public User? User { get; set; }
    }
}
