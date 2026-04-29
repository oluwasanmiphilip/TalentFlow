using TalentFlow.Domain.Common;
using TalentFlow.Domain.Entities;
using TalentFlow.Domain.Events;

public class Course : EntityBase
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Status { get; private set; } = "draft";

    // New UX fields
    public string ThumbnailUrl { get; private set; } = string.Empty;
    public Guid InstructorId { get; private set; }
    public int DurationMinutes { get; private set; }
    public string Level { get; private set; } = "Beginner"; // Beginner/Intermediate/Advanced
    public decimal Price { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public double Rating { get; private set; } = 0.0;

    // Curriculum
    private readonly List<Lesson> _lessons = new();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

    // Audit fields
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    // Navigation
    public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
    public Instructor? Instructor { get; private set; }
    public Certificate? Certificate { get; private set; }

    private Course() { } // EF Core

    public Course(string title, string description, string slug, string thumbnailUrl,
                  Guid instructorId, int durationMinutes, string level, decimal price, List<string> tags)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Slug = slug;
        ThumbnailUrl = thumbnailUrl;
        InstructorId = instructorId;
        DurationMinutes = durationMinutes;
        Level = level;
        Price = price;
        Tags = tags;
        Status = "draft";
        AddDomainEvent(new CourseCreatedEvent(this));
    }

    public void UpdateDetails(
    string title,
    string description,
    string thumbnailUrl,
    Guid instructorId,
    int durationMinutes,
    string level,
    decimal price,
    List<string> tags,
    string updatedBy)
    {
        Title = title;
        Description = description;
        ThumbnailUrl = thumbnailUrl;
        InstructorId = instructorId;
        DurationMinutes = durationMinutes;
        Level = level;
        Price = price;
        Tags = tags;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new CourseUpdatedEvent(this));
    }

    public void Enroll(User user, string role = "Learner", Guid? firstLessonId = null)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        // Prevent duplicate enrollment
        if (Enrollments.Any(e => e.UserId == user.Id && !e.IsDeleted))
            return;

        Enrollments.Add(new Enrollment(Id, user.Id, role, firstLessonId));
    }

    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
    }


    // Existing methods (Publish, UpdateDetails, SoftDelete, Enroll) remain
}
