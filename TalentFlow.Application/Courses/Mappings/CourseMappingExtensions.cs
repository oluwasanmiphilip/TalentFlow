using TalentFlow.Domain.Entities;
using TalentFlow.Application.Courses.DTOs;

namespace TalentFlow.Application.Courses.Mappings
{
    public static class CourseMappingExtensions
    {
        public static CourseDto ToDto(this Course course)
        {
            return new CourseDto
            {
                Slug = course.Slug,
                Title = course.Title,
                Description = course.Description,
                Enrollments = course.Enrollments
                    .Select(e => new EnrollmentDto
                    {
                        UserId = e.UserId,
                        EnrolledAt = e.EnrolledAt
                    })
                    .ToList()
            };
        }
    }
}
