using System;
using System.Collections.Generic;
using MediatR;

namespace TalentFlow.Application.Enrollments.Queries
{
    public record GetEnrollmentsByCourseQuery(Guid CourseId) : IRequest<List<EnrollmentDto>>;
}
