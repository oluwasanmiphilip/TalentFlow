using MediatR;
using TalentFlow.Application.Users.DTOs;

public record CreateUserCommand(string LearnerId, string Email, string FullName)
    : IRequest<UserDto>;
