using MediatR;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Domain.Entities;

namespace TalentFlow.Application.Users.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Use the domain constructor instead of property setters
            var user = new User(request.LearnerId, request.Email, request.FullName);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map back to DTO (no internal Id exposed)
            return new UserDto
            {
                LearnerId = user.LearnerId,
                Email = user.Email,
                FullName = user.FullName
            };
        }
    }
}
