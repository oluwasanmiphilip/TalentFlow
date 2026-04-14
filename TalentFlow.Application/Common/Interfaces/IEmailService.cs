using System.Threading.Tasks;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string otpCode);
    }
}
