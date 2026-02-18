using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebApplication1.Hubs
{
    public class MessageHub : Hub
    {
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        public async Task NotifyNewMessage(string receiverId, object messageData)
        {
            await Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", messageData);
        }

        public async Task NotifyMessageRead(string senderId, int messageId)
        {
            await Clients.Group($"user_{senderId}").SendAsync("MessageRead", messageId);
        }
    }
}
