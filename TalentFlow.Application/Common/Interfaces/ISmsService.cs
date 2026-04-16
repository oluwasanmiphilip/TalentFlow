using System.Threading.Tasks;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ISmsService
    {
        Task SendAsync(string phoneNumber, string message);
        Task SendOtpAsync(string toPhoneNumber, string otpCode);
    }
}
