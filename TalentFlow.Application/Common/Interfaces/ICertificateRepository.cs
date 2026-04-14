using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ICertificateRepository
    {
        Task<List<Certificate>> GetCertificatesByLearnerIdAsync(Guid learnerId, CancellationToken ct = default);
        Task AddAsync(Certificate certificate, CancellationToken ct = default);
        Task<Certificate?> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct = default);
    }
}
