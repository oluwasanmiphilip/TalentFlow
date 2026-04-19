using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.Handlers
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public LoginUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLower();

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var isValidPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedAccessException("Invalid email or password");

            // ✅ FIX: Clear previous session token (allow re-login)
            user.LastLoginToken = null;
            await _userRepository.UpdateAsync(user, cancellationToken);

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}