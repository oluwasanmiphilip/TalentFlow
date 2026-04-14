using System;
using MediatR;
using TalentFlow.Application.CourseProgress.DTOs;

namespace TalentFlow.Application.CourseProgress.Queries
{
    public record GetCourseProgressQuery(Guid UserId, Guid CourseId)
        : IRequest<CourseProgressDto?>;
}
