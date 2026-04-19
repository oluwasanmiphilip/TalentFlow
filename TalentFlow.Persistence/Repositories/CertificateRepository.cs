using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<Certificate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Certificates.FindAsync(new object[] { id }, ct);
        }

        public async Task<List<Certificate>> GetCertificatesByLearnerIdAsync(Guid learnerId, CancellationToken ct = default)
        {
            return await _context.Certificates
                .Where(c => c.LearnerId == learnerId)
                .ToListAsync(ct);
        }


        public async Task<List<Certificate>> GetByCourseAsync(Guid courseId, CancellationToken ct = default)
        {
            return await _context.Certificates
                .Where(c => c.CourseId == courseId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Certificate certificate, CancellationToken ct = default)
        {
            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
            await _context.Certificates.AddAsync(certificate, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var certificate = await _context.Certificates.FindAsync(new object[] { id }, ct);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
            }
        }
    }
}
