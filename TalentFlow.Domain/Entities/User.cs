using TalentFlow.Domain.Common;
using TalentFlow.Domain.Events;

namespace TalentFlow.Domain.Entities
{
    public class User : EntityBase
    {
        public Guid Id { get; private set; } // internal DB ID

        public Guid LearnerId { get; private set; }   // ✅ Guid now
        public string Email { get; private set; } = null!;
        public string FullName { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string Role { get; private set; } = null!;

        private User() { } // EF Core

        public User(Guid learnerId, string email, string name, string passwordHash, string role)
        {
            if (learnerId == Guid.Empty)
                throw new ArgumentException("LearnerId cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Full name cannot be empty");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("PasswordHash cannot be empty");

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role cannot be empty");

            Id = Guid.NewGuid();
            LearnerId = learnerId;
            Email = email;
            FullName = name;
            PasswordHash = passwordHash;
            Role = role;

            AddDomainEvent(new UserRegisteredDomainEvent(this));
        }

        public void UpdateProfile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Full name cannot be empty");

            FullName = name;
            AddDomainEvent(new UserProfileUpdatedDomainEvent(this));
        }
    }
}
