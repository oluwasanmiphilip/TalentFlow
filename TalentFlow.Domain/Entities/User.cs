using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

namespace TalentFlow.Domain.Entities
{
    [Table("users")]
    public class User : EntityBase
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = "Learner";

        // ✅ Single-session enforcement
        public string? LastLoginToken { get; set; }

        // Business identifiers
        public string LearnerId { get; private set; } =
            $"TM-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString().Substring(0, 6)}";

        // Profile fields
        public string Discipline { get; private set; } = string.Empty;
        public int CohortYear { get; private set; }
        public string? ProfilePhotoUrl { get; private set; }
        public string? Bio { get; private set; }
        public string ProgressVisibility { get; private set; } = "private";
        public string NotificationPrefs { get; private set; } = "{}"; // JSONB

        // ✅ Added PhoneNumber for OTP delivery
        public string PhoneNumber { get; private set; } = string.Empty;

        // Audit fields
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }
        public bool IsDeleted { get; private set; }
        public string? DeletedBy { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool EmailNotifications { get; set; }
        public void AttachProfile(ProfileUser profile)
        {
            ProfileUser = profile;
        }

        public ProfileUser? ProfileUser { get; private set; }


        private User() { } // EF Core

        public User(string email, string fullName, string passwordHash, string role, string discipline, int cohortYear, string phoneNumber)
        {
            Id = Guid.NewGuid();
            Email = email.ToLowerInvariant().Trim();
            FullName = fullName;
            PasswordHash = passwordHash;
            Role = role;
            Discipline = discipline;
            CohortYear = cohortYear;
            PhoneNumber = phoneNumber;

            AddDomainEvent(new UserCreatedEvent(this));
        }

        public void UpdateProfile(string fullName, string email, string phoneNumber, string updatedBy)
        {
            FullName = fullName;
            Email = email.ToLowerInvariant().Trim();
            PhoneNumber = phoneNumber;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;

            AddDomainEvent(new UserProfileUpdatedEvent(this));
        }

        public void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
