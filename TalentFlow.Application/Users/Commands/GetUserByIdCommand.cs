// File Path: TalentFlow.Application/Users/Commands/GetUserByIdCommand.cs
using System;
using MediatR;

namespace TalentFlow.Application.Users.Commands
{
    public class GetUserByIdCommand : IRequest<UserDto?>
    {
        public Guid UserId { get; set; }
    }
}
