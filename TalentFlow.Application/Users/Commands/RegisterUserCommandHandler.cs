using System.Net.Mail;
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

        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // ✅ Email format check
            try
            {
                var mailAddress = new MailAddress(request.Email);
            }
            catch
            {
                throw new Exception("Invalid email address format");
            }

            // ✅ Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists");
            }

            // ✅ Password rules
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters long");
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

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
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
