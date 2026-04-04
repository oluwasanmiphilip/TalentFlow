using Microsoft.EntityFrameworkCore;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;
using TalentFlow.Persistence;

namespace TalentFlow.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TalentFlowDbContext _context;

        public UserRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<User?> GetByLearnerIdAsync(string learnerId, CancellationToken ct) =>
            _context.Users.FirstOrDefaultAsync(u => u.LearnerId == learnerId, ct);

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
            _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public Task UpdateAsync(User user, CancellationToken ct)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }
    }
}
