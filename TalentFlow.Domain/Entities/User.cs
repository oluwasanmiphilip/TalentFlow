using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

namespace TalentFlow.Domain.Entities
{
    [Table("user")] // matches EF query
    public class User : EntityBase
    {
        // Primary key
        public Guid Id { get; private set; }

        // Business identifiers
        public Guid LearnerId { get; private set; } = Guid.NewGuid(); // ✅ Now a Guid
        public string Email { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;

        // Security
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = "Learner";

        // Audit fields
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }

        // Soft delete fields
        public bool IsDeleted { get; private set; }
        public string? DeletedBy { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        // EF Core constructor
        private User() { }

        // Domain constructor
        public User(string email, string fullName, string passwordHash, string role)
        {
            Id = Guid.NewGuid();          // PK
            LearnerId = Guid.NewGuid();   // ✅ Business identifier as Guid
            Email = email;
            FullName = fullName;
            PasswordHash = passwordHash;
            Role = role;

            AddDomainEvent(new UserCreatedDomainEvent(this));
        }

        // Update profile
        public void UpdateProfile(string fullName, string email, string updatedBy)
        {
            FullName = fullName;
            Email = email;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;

            AddDomainEvent(new UserProfileUpdatedDomainEvent(this));
        }

        // Soft delete
        public void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
