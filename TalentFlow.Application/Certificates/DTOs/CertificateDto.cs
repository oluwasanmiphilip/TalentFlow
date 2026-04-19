using System;

namespace TalentFlow.Application.Certificates.DTOs
{
    public class CertificateDto
    {
        public Guid Id { get; set; }
        public Guid LearnerId { get; set; }
        public Guid CourseId { get; set; }
        public string IssuedBy { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.MinValue;
        public string CertificateUrl { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }
}