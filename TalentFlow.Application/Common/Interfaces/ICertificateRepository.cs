using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ICertificateRepository
    {
        // Return all certificates for a learner
        Task<List<Certificate>> GetCertificatesByLearnerIdAsync(Guid learnerId, CancellationToken ct = default);

        // Add a new certificate
        Task AddAsync(Certificate certificate, CancellationToken ct = default);

        // Optional: fetch a single certificate by ID
        Task<Certificate?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
