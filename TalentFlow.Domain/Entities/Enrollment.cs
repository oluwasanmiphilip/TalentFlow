using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

[Table("enrollment")]
public class Enrollment : EntityBase
{
    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public string Role { get; private set; } = "Learner";
    public Guid? FirstLessonId { get; private set; }

    // Audit fields
    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private Enrollment() { } // EF Core

    public Enrollment(Guid courseId, Guid userId, string role = "Learner", Guid? firstLessonId = null)
    {
        Id = Guid.NewGuid();
        CourseId = courseId;
        UserId = userId;
        Role = role;
        FirstLessonId = firstLessonId;
        EnrolledAt = DateTime.UtcNow;

        AddDomainEvent(new UserEnrolledEvent(this));
    }

    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;

        AddDomainEvent(new EnrollmentWithdrawnEvent(this));
    }

    public void Update(string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }
}
