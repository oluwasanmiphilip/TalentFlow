using MediatR;


public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
