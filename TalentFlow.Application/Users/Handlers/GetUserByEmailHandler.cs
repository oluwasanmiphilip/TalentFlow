using MediatR;
using TalentFlow.Application.Users.Commands;
using TalentFlow.Application.Common.Models;

public class GetUserByEmailHandler
    : IRequestHandler<GetUserByEmailCommand, UserDto>
{
    public async Task<UserDto> Handle(GetUserByEmailCommand request, CancellationToken cancellationToken)
    {
        // your logic here
        return null;
    }
}