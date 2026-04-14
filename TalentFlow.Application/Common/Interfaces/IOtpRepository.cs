using System;
using System.Threading;
using System.Threading.Tasks;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IOtpRepository
    {
        Task<OtpCode?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(OtpCode otp);
        Task InvalidateAsync(Guid userId, CancellationToken cancellationToken);
        Task MarkAsUsedAsync(Guid userId, CancellationToken cancellationToken);
    }
}
