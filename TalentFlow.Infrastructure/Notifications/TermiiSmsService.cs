using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Notifications
{
    public class TermiiSmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _senderId;

        public TermiiSmsService(HttpClient httpClient, string apiKey, string senderId)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _senderId = senderId;
        }

        public async Task SendOtpAsync(string toPhoneNumber, string otpCode)
        {
            var payload = new
            {
                api_key = _apiKey,
                to = toPhoneNumber,
                from = _senderId,
                sms = $"Your TalentFlow OTP code is {otpCode}",
                type = "plain",
                channel = "generic"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("https://api.ng.termii.com/api/sms/send", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
