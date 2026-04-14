using System.Threading.Tasks;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface ISmsService
    {
        Task SendOtpAsync(string toPhoneNumber, string otpCode);
    }
}
