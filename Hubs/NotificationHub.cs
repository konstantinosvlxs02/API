using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApplication1.Hubs
{
    public class NotificationHub : Hub
    {
        // Called when client connects
        public async Task JoinUserNotifications(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"notifications_{userId}");
        }

        // Called when client disconnects
        public async Task LeaveUserNotifications(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"notifications_{userId}");
        }

        // Method to send notification to specific user
        public async Task SendNotificationToUser(string userId, object notificationData)
        {
            await Clients.Group($"notifications_{userId}").SendAsync("ReceiveNotification", notificationData);
        }
    }
}
