using TalentFlow.Domain.Common;



namespace TalentFlow.Domain.Entities
{
    public class User : EntityBase
    {
        public Guid Id { get; private set; } // internal DB ID, never exposed
        public string LearnerId { get; private set; } // public identifier
        public string Email { get; private set; }
        public string FullName { get; private set; }

        private User() { } // EF Core

        public User(string learnerId, string email, string name)
        {
            Id = Guid.NewGuid();
            LearnerId = learnerId;
            Email = email;
            FullName = name;

            // Raise domain-level event
            AddDomainEvent(new TalentFlow.Domain.Events.UserCreatedDomainEvent(this));
        }

        public void UpdateProfile(string name)
        {
            FullName = name;
            AddDomainEvent(new TalentFlow.Domain.Events.UserProfileUpdatedDomainEvent(this));
        }

    }
}
