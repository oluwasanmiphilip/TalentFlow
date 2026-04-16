using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TalentFlow.Application.Common.Interfaces;

namespace TalentFlow.Infrastructure.Email
{
    public class BrevoEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public BrevoEmailService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task SendOtpAsync(string toEmail, string otpCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");

            request.Headers.Add("api-key", _apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var body = new
            {
                sender = new { name = "TalentFlow", email = "no-reply@talentflow.com" },
                to = new[] { new { email = toEmail } },
                subject = "Your OTP Code",
                htmlContent = $"<p>Your OTP is <strong>{otpCode}</strong>. Expires in 5 minutes.</p>"
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Brevo Email Failed: {error}");
            }
        }
    }
}