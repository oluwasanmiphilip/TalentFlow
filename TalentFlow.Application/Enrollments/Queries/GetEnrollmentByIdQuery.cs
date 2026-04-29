using MediatR;

public record GetEnrollmentByIdQuery(Guid Id) : IRequest<EnrollmentDto?>;
