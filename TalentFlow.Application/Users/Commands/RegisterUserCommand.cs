// File: src/TalentFlow.Application/Users/Commands/RegisterUserCommand.cs
using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class RegisterUserCommand : IRequest<UserDto?>
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty;
        public int CohortYear { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;
        public bool? EmailNotifications { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? Bio { get; set; }
    }
}
