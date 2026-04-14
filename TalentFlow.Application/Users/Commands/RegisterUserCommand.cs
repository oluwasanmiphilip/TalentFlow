using MediatR;
using TalentFlow.Application.Users.DTOs;

namespace TalentFlow.Application.Users.Commands
{
    public class RegisterUserCommand : IRequest<UserDto>
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty;
        public int CohortYear { get; set; }   // ✅ int, matches User entity

        // ✅ Added PhoneNumber to match User constructor
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
