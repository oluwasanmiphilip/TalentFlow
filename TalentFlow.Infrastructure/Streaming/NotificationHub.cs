using Microsoft.AspNetCore.SignalR;

namespace TalentFlow.Infrastructure.Streaming
{
    public class NotificationHub : Hub
    {
        public async Task SendProfileUpdate(string learnerId, string message)
        {
            await Clients.User(learnerId).SendAsync("ProfileUpdated", message);
        }
    }
}
