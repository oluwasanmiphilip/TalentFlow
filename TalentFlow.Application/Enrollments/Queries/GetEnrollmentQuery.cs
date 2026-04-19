using MediatR;
using System;
using TalentFlow.Application.Enrollments.DTOs;

namespace TalentFlow.Application.Enrollments.Queries
{
    public record GetEnrollmentQuery(Guid UserId, Guid CourseId)
        : IRequest<EnrollmentDto>;
}