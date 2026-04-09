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

            var user = new User(
                request.Email,
                request.FullName,
                passwordHash,
                request.Role
            );

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
