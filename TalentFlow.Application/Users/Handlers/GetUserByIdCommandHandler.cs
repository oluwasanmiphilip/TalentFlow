// File Path: TalentFlow.Application/Users/Handlers/GetUserByIdCommandHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Users.DTOs;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Users.Handlers
{
    public class GetUserByIdCommandHandler : IRequestHandler<GetUserByIdCommand, UserDto?>
    {
        private readonly IUserRepository _userRepo;

        public GetUserByIdCommandHandler(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserDto?> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) return null;

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
