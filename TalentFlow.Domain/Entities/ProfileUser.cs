using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;

namespace TalentFlow.Domain.Entities
{
    [Table("profile_users")]
    public class ProfileUser : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        // Profile-specific fields
        public string Bio { get; private set; } = string.Empty;
        public string? ProfilePhotoUrl { get; private set; }
        public string ProgressVisibility { get; private set; } = "private";
        public string NotificationPrefs { get; private set; } = "{}";

        // Navigation
        public User User { get; private set; } = null!;

        private ProfileUser() { } // EF Core

        public ProfileUser(Guid userId, string bio, string? profilePhotoUrl, string progressVisibility, string notificationPrefs)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Bio = bio;
            ProfilePhotoUrl = profilePhotoUrl;
            ProgressVisibility = progressVisibility;
            NotificationPrefs = notificationPrefs;
        }
    }
}
