using System;
using MediatR;
using TalentFlow.Application.Certificates.DTOs;

namespace TalentFlow.Application.Certificates.Commands
{
    public class IssueCertificateCommand : IRequest<CertificateDto>
    {
        public Guid LearnerId { get; }
        public Guid CourseId { get; }
        public string IssuedBy { get; }
        public DateTime? ExpiresAt { get; }

        public IssueCertificateCommand(Guid learnerId, Guid courseId, string issuedBy, DateTime? expiresAt = null)
        {
            LearnerId = learnerId;
            CourseId = courseId;
            IssuedBy = issuedBy;
            ExpiresAt = expiresAt;
        }
    }
}
