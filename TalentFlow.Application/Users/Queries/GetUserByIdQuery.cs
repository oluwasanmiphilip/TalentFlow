using MediatR;
using TalentFlow.Application.Users.DTOs;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
