using MediatR;
using System;

namespace TalentFlow.Application.Enrollments.Queries
{
    public record GetEnrollmentQuery(Guid UserId, Guid CourseId)
        : IRequest<EnrollmentDto>;
}