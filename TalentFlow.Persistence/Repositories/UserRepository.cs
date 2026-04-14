using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TalentFlowDbContext _context;

        public UserRepository(TalentFlowDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);
        }

        public async Task<User?> GetByLearnerIdAsync(string learnerId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.LearnerId == learnerId && !u.IsDeleted, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(User user, CancellationToken ct = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }

        public async Task SoftDeleteAsync(User user, CancellationToken ct = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.SoftDelete(user.DeletedBy ?? "system");
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
        }
    }
}
