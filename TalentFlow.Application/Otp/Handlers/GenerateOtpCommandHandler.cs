using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TalentFlow.Application.Otp.Commands;

namespace TalentFlow.Application.Otp.Handlers
{
    public class GenerateOtpCommandHandler : IRequestHandler<GenerateOtpCommand, string>
    {
        public Task<string> Handle(GenerateOtpCommand request, CancellationToken cancellationToken)
        {
            // Example: generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            return Task.FromResult(otp);
        }
    }
}
