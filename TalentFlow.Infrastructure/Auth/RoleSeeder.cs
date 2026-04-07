using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Infrastructure.Auth
{
    public class RoleSeeder
    {
        private readonly IRoleRepository _roleRepository;

        public RoleSeeder(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "Instructor", "Learner" };

            foreach (var roleName in roles)
            {
                var existingRole = await _roleRepository.GetByNameAsync(roleName, CancellationToken.None);

                if (existingRole == null)
                {
                    var role = new Role(roleName);   // ✅ use constructor
                    await _roleRepository.AddAsync(role, CancellationToken.None);
                }
            }
        }
    }
}
