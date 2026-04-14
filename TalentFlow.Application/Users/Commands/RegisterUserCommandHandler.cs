using System.Threading;
using System.Threading.Tasks;
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
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // ✅ Pass phoneNumber as the last argument
            var user = new User(
                request.Email,
                request.FullName,
                passwordHash,
                request.Role,
                request.Discipline,
                request.CohortYear,
                request.PhoneNumber
            );

            await _userRepository.AddAsync(user, cancellationToken);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Discipline = user.Discipline,
                CohortYear = user.CohortYear,
                PhoneNumber = user.PhoneNumber // ✅ include in DTO
            };
        }
    }
}
