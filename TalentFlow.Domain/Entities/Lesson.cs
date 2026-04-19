using System;

namespace TalentFlow.Domain.Entities
{
    public class Lesson
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }

        public string Title { get; private set; }
        public string Content { get; private set; }
        public string ContentUrl { get; private set; }   // ✅ Added for video/PDF/quiz playback

        public int Order { get; private set; }
        public TimeSpan Duration { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Navigation
        public Course? Course { get; private set; }

        private Lesson() { } // EF Core

        public Lesson(Guid courseId, string title, string content, string contentUrl, int order, TimeSpan duration)
        {
            Id = Guid.NewGuid();
            CourseId = courseId;
            Title = title;
            Content = content;
            ContentUrl = contentUrl;
            Order = order;
            Duration = duration;

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string title, string content, string contentUrl, int order, TimeSpan duration)
        {
            Title = title;
            Content = content;
            ContentUrl = contentUrl;
            Order = order;
            Duration = duration;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
