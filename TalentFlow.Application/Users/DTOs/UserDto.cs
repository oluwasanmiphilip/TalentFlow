using System;

namespace TalentFlow.Application.Users.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty;
        public int CohortYear { get; set; }   // ✅ matches entity

        // ✅ Added PhoneNumber to match User entity
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
