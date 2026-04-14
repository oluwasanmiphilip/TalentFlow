using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

namespace TalentFlow.Domain.Entities
{
    [Table("notification")]
    public class Notification : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Message { get; private set; } = string.Empty;

        // Audit fields
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }

        // Soft delete fields
        public bool IsDeleted { get; private set; } = false;
        public string? DeletedBy { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        // Sent status
        public bool IsSent { get; private set; } = false;
        public DateTime? SentAt { get; private set; }

        private Notification() { } // EF Core

        public Notification(Guid userId, string message)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            CreatedAt = DateTime.UtcNow;
        }

        // Soft delete helper
        public void SoftDelete(string deletedBy)
        {
            if (string.IsNullOrWhiteSpace(deletedBy))
                throw new ArgumentException("DeletedBy cannot be null or empty", nameof(deletedBy));

            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedAt = DateTime.UtcNow;
        }

        // Mark as sent helper
        public void MarkAsSent()
        {
            IsSent = true;
            SentAt = DateTime.UtcNow;
            AddDomainEvent(new NotificationSentEvent(this));
        }

        // Mark as read helper
        public void MarkAsRead(string updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new NotificationReadEvent(this));
        }

        // Optional update helper
        public void Update(string message, string updatedBy)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            Message = message;
            UpdatedBy = updatedBy;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
