using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

namespace TalentFlow.Domain.Entities
{
    [Table("course")]
    public class Course : EntityBase
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string Status { get; private set; } = "draft"; // draft/published
        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;


        // Curriculum structure
        private readonly List<Lesson> _lessons = new();
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        // Audit fields
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? DeletedBy { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Course() { } // EF Core

        // ✅ Add navigation property
        public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();


        public Course(string title, string description, string slug)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Slug = slug;
            Status = "draft";
            AddDomainEvent(new CourseCreatedEvent(this));
        }

        public void Publish(string updatedBy)
        {
            Status = "published";
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CoursePublishedEvent(this));
        }

        public void UpdateDetails(string title, string description, string updatedBy)
        {
            Title = title;
            Description = description;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CourseUpdatedEvent(this));
        }

        public void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
        }
        // ✅ Add enroll method
        public void Enroll(User user, string role = "Learner", Guid? firstLessonId = null)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Prevent duplicate enrollment
            if (Enrollments.Any(e => e.UserId == user.Id))
                return;

            Enrollments.Add(new Enrollment(Id, user.Id, role, firstLessonId));
        }

    }
}
