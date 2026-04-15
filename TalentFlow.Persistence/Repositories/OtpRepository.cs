using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class OtpRepository : IOtpRepository
    {
        private readonly TalentFlowDbContext _context;

        public OtpRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public async Task<OtpCode?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.UserId == userId && !o.IsUsed, cancellationToken);
        }

        public async Task AddAsync(OtpCode otp)
        {
            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateAsync(Guid userId, CancellationToken cancellationToken)
        {
            var codes = await _context.OtpCodes
                .Where(o => o.UserId == userId && !o.IsUsed)
                .ToListAsync(cancellationToken);

            foreach (var code in codes)
                code.MarkUsed();

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarkAsUsedAsync(Guid userId, CancellationToken cancellationToken)
        {
            var otp = await _context.OtpCodes
                .FirstOrDefaultAsync(o => o.UserId == userId && !o.IsUsed, cancellationToken);

            if (otp != null)
            {
                otp.MarkUsed();
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
