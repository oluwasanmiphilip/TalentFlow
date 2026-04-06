using TalentFlow.Domain.Common;

namespace TalentFlow.Domain.Entities
{
    public class Role : EntityBase
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }

        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private Role() { } // EF Core

        public Role(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public void AssignUser(User user)
        {
            _users.Add(user);
        }
    }
}
