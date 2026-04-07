using System;

namespace TalentFlow.Application.Users.DTOs
{
    public class UserDto
    {
        public Guid LearnerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
