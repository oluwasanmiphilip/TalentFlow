using MediatR;
using TalentFlow.Application.Enrollments.DTOs;

public record GetEnrollmentByIdQuery(Guid Id) : IRequest<EnrollmentDto?>;
