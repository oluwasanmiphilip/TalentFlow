using MediatR;
using TalentFlow.Application.Roles.Commands;
using TalentFlow.Application.Roles.DTOs;
using TalentFlow.Domain.Entities;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Application.Roles.Handlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role(request.Name);

            // Persist via RoleRepository inside UnitOfWork
            await _unitOfWork.Roles.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RoleDto { Id = role.Id, Name = role.Name };
        }
    }
}
