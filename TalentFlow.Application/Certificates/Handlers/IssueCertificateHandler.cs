using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Certificates.Commands;
using TalentFlow.Application.Certificates.DTOs;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Mappings;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Certificates.Handlers
{
    public class IssueCertificateHandler : IRequestHandler<IssueCertificateCommand, CertificateDto>
    {
        private readonly ICertificateRepository _certificateRepository;

        public IssueCertificateHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<CertificateDto> Handle(IssueCertificateCommand request, CancellationToken cancellationToken)
        {
            // Create new certificate entity
            var certificate = new Certificate(request.LearnerId, request.CourseId, request.IssuedBy, request.ExpiresAt);

            // Populate the CertificateUrl (example: CDN or blob storage path)
            certificate.CertificateUrl = $"https://cdn.talentflow.com/certificates/{certificate.Id}.pdf";

            // Save to repository
            await _certificateRepository.AddAsync(certificate, cancellationToken);

            // Return DTO
            return certificate.ToDto();
        }
    }
}
