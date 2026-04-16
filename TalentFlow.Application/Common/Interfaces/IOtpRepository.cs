using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IOtpRepository
    {
        Task<OtpCode?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<OtpCode?> GetByUserIdAndCodeAsync(Guid userId, string code);
        Task<IEnumerable<OtpCode>> GetActiveOtpsByUserIdAsync(Guid userId);
        Task AddAsync(OtpCode otp);
        Task UpdateAsync(OtpCode otp);
        Task InvalidateAsync(Guid userId, CancellationToken cancellationToken);
        Task MarkAsUsedAsync(Guid userId, CancellationToken cancellationToken);
    }
}
