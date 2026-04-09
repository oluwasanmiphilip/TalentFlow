using MediatR;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Users.Commands
{
    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Use domain constructor (LearnerId generated internally)
            var user = new User(
                request.Email,
                request.FullName,
                passwordHash,
                request.Role
            );

            await _userRepository.AddAsync(user, cancellationToken);

            return new UserDto
            {
                Id = user.Id,
                //LearnerId = user.LearnerId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            };
        }
    }
}
