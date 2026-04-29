using TalentFlow.Domain.Common;

public class Enrollment : EntityBase
{
    public Guid Id { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; } = "Learner";
    public string Status { get; private set; } = "Active";

    public DateTime EnrolledAt { get; private set; }   // ✅ NEW

    public string? UpdatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public string? DeletedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Enrollment() { } // EF Core

    public Enrollment(Guid courseId, Guid userId, string role, Guid? firstLessonId = null)
    {
        Id = Guid.NewGuid();
        CourseId = courseId;
        UserId = userId;
        Role = role;
        Status = "Active";
        EnrolledAt = DateTime.UtcNow;   // ✅ set when created
    }

    public void Update(string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = DateTime.UtcNow;
        Status = "Withdrawn";
    }

    public void ChangeStatus(string status, string updatedBy)
    {
        Status = status;
        Update(updatedBy);
    }
}
