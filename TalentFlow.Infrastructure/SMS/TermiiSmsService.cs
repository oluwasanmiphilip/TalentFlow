using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Sms
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

        // Generic SMS sender
        public async Task SendAsync(string phoneNumber, string message)
        {
            var payload = new
            {
                api_key = _apiKey,
                to = phoneNumber,
                from = _senderId,
                sms = message,
                type = "plain",
                channel = "generic"
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.ng.termii.com/api/sms/send", content);
            response.EnsureSuccessStatusCode();
        }

        // OTP-specific helper
        public async Task SendOtpAsync(string toPhoneNumber, string otpCode)
        {
            var message = $"Your OTP code is {otpCode}. It expires in 5 minutes.";
            await SendAsync(toPhoneNumber, message);
        }
    }
}
