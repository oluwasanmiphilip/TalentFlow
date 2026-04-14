using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;

namespace TalentFlow.Domain.Entities
{
    [Table("lesson")] // matches EF query
    public class Lesson : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Content { get; private set; } = string.Empty;
        public int Order { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime CreatedAt { get; private set; }
        private Lesson() { } // EF Core

        public Lesson(Guid courseId, string title, string content, int order, TimeSpan duration)
        {
            Id = Guid.NewGuid();
            CourseId = courseId;
            Title = title;
            Content = content;
            Order = order;
            Duration = duration;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string title, string content, int order, TimeSpan duration)
        {
            Title = title;
            Content = content;
            Order = order;
            Duration = duration;
        }
    }
}
