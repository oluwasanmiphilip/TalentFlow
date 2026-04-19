using System;
using System.ComponentModel.DataAnnotations.Schema;
using TalentFlow.Domain.Common;

namespace TalentFlow.Domain.Entities
{
    [Table("certificate")] // matches EF query
    public class Certificate : EntityBase
    {
        public Guid Id { get; private set; }
        public Guid LearnerId { get; private set; }
        public Guid CourseId { get; private set; }
        public DateTime IssuedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public string IssuedBy { get; private set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public Certificate(Guid learnerId, Guid courseId, string issuedBy, DateTime? expiresAt = null)
        {
            Id = Guid.NewGuid();
            LearnerId = learnerId;
            CourseId = courseId;
            IssuedBy = issuedBy;
            IssuedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
        }
    }
}