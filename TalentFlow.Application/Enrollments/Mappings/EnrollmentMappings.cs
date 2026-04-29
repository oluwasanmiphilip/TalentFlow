// Application/Enrollments/Mappings/EnrollmentMappings.cs
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Enrollments.Mappings
{
    public static class EnrollmentMappings
    {
        public static EnrollmentDto ToDto(this Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                CourseId = enrollment.CourseId,
                UserId = enrollment.UserId,
                EnrolledAt = enrollment.EnrolledAt,   // ✅ matches entity
                UpdatedBy = enrollment.UpdatedBy,
                UpdatedAt = enrollment.UpdatedAt,
                DeletedBy = enrollment.DeletedBy,
                DeletedAt = enrollment.DeletedAt,
                IsDeleted = enrollment.IsDeleted
            };
        }
    }
}
