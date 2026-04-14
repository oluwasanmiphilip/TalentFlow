using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly TalentFlowDbContext _context;

        public CertificateRepository(TalentFlowDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Certificate>> GetCertificatesByLearnerIdAsync(Guid learnerId, CancellationToken ct = default)
        {
            return await _context.Certificates
                .Where(cert => cert.LearnerId == learnerId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Certificate certificate, CancellationToken ct = default)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            await _context.Certificates.AddAsync(certificate, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Certificate?> GetByLearnerIdAsync(Guid learnerId, CancellationToken ct = default)
        {
            return await _context.Certificates
                .FirstOrDefaultAsync(cert => cert.LearnerId == learnerId, ct);
        }
    }
}
