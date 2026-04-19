using System;

namespace TalentFlow.Domain.Entities
{
    public class LessonProgress
    {
        public Guid Id { get; private set; }
        public Guid LessonId { get; private set; }
        public Guid UserId { get; private set; }

        // Track video position in seconds
        public int VideoPositionSeconds { get; private set; }

        // Completion timestamp
        public DateTime? CompletedAt { get; private set; }

        // Course-level progress percentage
        public decimal CoursePercentage { get; private set; }

        // Raw playback position
        public TimeSpan? VideoPosition { get; private set; }

        // Navigation
        public Lesson? Lesson { get; private set; }
        public User? User { get; private set; }

        private LessonProgress() { } // EF Core

        public LessonProgress(Guid userId, Guid lessonId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            LessonId = lessonId;
            VideoPositionSeconds = 0;
            CoursePercentage = 0;
        }

        // Update progress with percentage + playback position
        public void UpdateProgress(decimal percentage, TimeSpan? position)
        {
            CoursePercentage = percentage;
            VideoPosition = position;
            VideoPositionSeconds = position.HasValue ? (int)position.Value.TotalSeconds : 0;
        }

        // Mark lesson complete
        public void MarkComplete()
        {
            CoursePercentage = 100;
            CompletedAt = DateTime.UtcNow;
        }
    }
}
