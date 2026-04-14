using System;
using System.Linq;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly TalentFlowDbContext _context;

        public RefreshTokenRepository(TalentFlowDbContext context)
        {
            _context = context;
        }

        public RefreshToken? GetByToken(string token) =>
            _context.RefreshTokens.FirstOrDefault(r => r.Token == token);

        public void Save(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();
        }

        public void Revoke(string token)
        {
            var rt = GetByToken(token);
            if (rt != null)
            {
                rt.IsRevoked = true;
                _context.SaveChanges();
            }
        }

        public void RevokeAllForUser(Guid userId)
        {
            var tokens = _context.RefreshTokens.Where(r => r.UserId == userId).ToList();
            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }
            _context.SaveChanges();
        }
    }
}
