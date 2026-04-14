using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Certificates.DTOs;
using TalentFlow.Application.Certificates.Queries;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Common.Mappings; // ✅ FIX

namespace TalentFlow.Application.Certificates.Handlers
{
    public class GetCertificatesByUserIdHandler
        : IRequestHandler<GetCertificatesByUserIdQuery, List<CertificateDto>>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificatesByUserIdHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<List<CertificateDto>> Handle(
    GetCertificatesByUserIdQuery request,
    CancellationToken cancellationToken)
        {
            var certificates = await _certificateRepository
                .GetCertificatesByLearnerIdAsync(request.UserId, cancellationToken);

            return certificates.Select(c => c.ToDto()).ToList();
        }

    }
}