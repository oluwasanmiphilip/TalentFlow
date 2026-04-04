using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public record UpdateUserProfileCommand(string LearnerId, string Name) : IRequest<bool>;
}
