using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return new UserDto
            {
                Id = user.Id,
                //LearnerId = user.LearnerId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}
