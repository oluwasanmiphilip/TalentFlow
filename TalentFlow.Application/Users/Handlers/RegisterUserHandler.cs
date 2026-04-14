using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Users.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var passwordHash = _passwordHasher.Hash(request.Password);

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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // ✅ Return all relevant fields including PhoneNumber
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
