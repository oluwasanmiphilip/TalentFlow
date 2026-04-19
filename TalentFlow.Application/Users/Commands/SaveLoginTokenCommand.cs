using MediatR;
using System;

namespace TalentFlow.Application.Users.Commands
{
    public class SaveLoginTokenCommand : IRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}