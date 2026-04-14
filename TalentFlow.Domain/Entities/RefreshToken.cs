using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentFlow.Domain.Entities
{
    [Table("refresh_tokens")] // ✅ pluralized table name for clarity
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Token { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string Role { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;
    }
}
