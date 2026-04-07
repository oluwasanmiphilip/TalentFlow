using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class UpdateUserProfileCommand : IRequest<bool>
    {
        public Guid LearnerId { get; set; }   // ✅ Guid
        public string Name { get; set; } = string.Empty;
    }
}
