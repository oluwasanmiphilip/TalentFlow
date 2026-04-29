using System;
using System.Collections.Generic;
using MediatR;

namespace TalentFlow.Application.Courses.Queries
{
    public record GetCourseEnrollmentQuery(Guid CourseId) : IRequest<List<EnrollmentDto>>;
}
