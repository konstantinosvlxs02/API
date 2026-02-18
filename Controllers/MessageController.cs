using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Hubs;
using Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly Database _context;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public MessageController(Database context, IHubContext<MessageHub> hubContext, IHubContext<NotificationHub> notificationHubContext)
        {
            _context = context;
            _hubContext = hubContext;
            _notificationHubContext = notificationHubContext;
        }

        // GET: Message - Show conversations list
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var sentToUserIds = _context.Messages
                .Where(m => m.SenderId == userId)
                .SelectMany(m => m.Receivers.Select(r => r.ReceiverId))
                .ToList();

            var receivedFromUserIds = _context.Messages
                .Where(m => m.Receivers.Any(r => r.ReceiverId == userId))
                .Select(m => m.SenderId)
                .ToList();

            var allUserIds = sentToUserIds.Concat(receivedFromUserIds)
                .Distinct()
                .Where(id => id != userId)
                .ToList();

            var conversations = new List<dynamic>();
            foreach (var otherUserId in allUserIds)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == otherUserId);
                if (user == null) continue;

                var unreadCount = _context.Receivers
                    .Include(r => r.Message)
                    .Count(r => r.ReceiverId == userId &&
                               r.Message.SenderId == otherUserId &&
                               !r.IsRead);

                var lastMsg = _context.Messages
                    .Include(m => m.Receivers)
                    .Where(m =>
                        (m.SenderId == userId && m.Receivers.Any(r => r.ReceiverId == otherUserId)) ||
                        (m.SenderId == otherUserId && m.Receivers.Any(r => r.ReceiverId == userId))
                    )
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new { m.Content, m.CreatedAt })
                    .FirstOrDefault();

                conversations.Add(new
                {
                    UserId = otherUserId,
                    Username = user.Username,
                    UnreadCount = unreadCount,
                    LastMessage = lastMsg?.Content ?? "",
                    LastMessageTime = lastMsg?.CreatedAt ?? DateTime.MinValue
                });
            }

            ViewBag.Conversations = conversations.Cast<dynamic>().ToList();
            ViewBag.Contacts = _context.Contact
                .Where(c => c.UserId == userId)
                .Include(c => c.ContactUser)
                .ToList();

            return View();
        }

        // GET: Message/Conversation/5 - Show chat with specific user
        public IActionResult Conversation(int userId)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var otherUser = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (otherUser == null)
                return NotFound();

            var messages = _context.Messages
                .Include(m => m.Receivers)
                .Include(m => m.Sender)
                .Where(m =>
                    (m.SenderId == currentUserId && m.Receivers.Any(r => r.ReceiverId == userId)) ||
                    (m.SenderId == userId && m.Receivers.Any(r => r.ReceiverId == currentUserId))
                )
                .OrderBy(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.SenderId,
                    SenderName = m.Sender.Username,
                    m.CreatedAt,
                    IsSentByMe = m.SenderId == currentUserId
                })
                .ToList();

            // Mark messages as read
            var unreadReceivers = _context.Receivers
                .Include(r => r.Message)
                .Where(r => r.ReceiverId == currentUserId && r.Message.SenderId == userId && !r.IsRead)
                .ToList();

            foreach (var r in unreadReceivers)
            {
                r.IsRead = true;
            }
            _context.SaveChanges();

            ViewBag.OtherUser = otherUser;
            ViewBag.Messages = messages.Cast<dynamic>().ToList();
            ViewBag.CurrentUserId = currentUserId;

            return View();
        }

        // POST: Message/Send - Send a message (supports both AJAX and form post)
        [HttpPost]
        public async Task<IActionResult> Send(string content, string receiverIds)
        {
            int senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(receiverIds))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest("Missing content or receivers");
                TempData["Error"] = "Παρακαλώ συμπληρώστε το μήνυμα.";
                return RedirectToAction("Index");
            }

            var ids = receiverIds.Split(',').Select(int.Parse).ToList();

            var message = new Message
            {
                Content = content,
                SenderId = senderId,
                Receivers = ids.Select(id => new Receivers { ReceiverId = id }).ToList()
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var sender = _context.Users.FirstOrDefault(u => u.Id == senderId);
            var senderName = sender?.Username ?? "Someone";

            var messageData = new
            {
                message.Id,
                message.Content,
                message.SenderId,
                message.CreatedAt,
                ReceiverIds = ids
            };

            foreach (var receiverId in ids)
            {
                await _hubContext.Clients.Group($"user_{receiverId}")
                    .SendAsync("ReceiveMessage", messageData);

                var notification = new Notification
                {
                    UserId = receiverId,
                    Title = $"Νέο μήνυμα από {senderName}",
                    Content = message.Content.Length > 50 ? message.Content.Substring(0, 50) + "..." : message.Content,
                    Type = "message",
                    RelatedEntityId = message.Id
                };

                _context.Notifications.Add(notification);

                await _notificationHubContext.Clients.Group($"notifications_{receiverId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        notification.Title,
                        notification.Content,
                        notification.Type,
                        notification.RelatedEntityId,
                        CreatedAt = DateTime.UtcNow
                    });
            }

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(messageData);

            if (ids.Count == 1)
                return RedirectToAction("Conversation", new { userId = ids[0] });

            TempData["Success"] = "Το μήνυμα στάλθηκε!";
            return RedirectToAction("Index");
        }

        // GET: Message/NewMessage - Form to send to multiple users
        public IActionResult NewMessage()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            ViewBag.Contacts = _context.Contact
                .Where(c => c.UserId == userId)
                .Include(c => c.ContactUser)
                .ToList();

            ViewBag.AllUsers = _context.Users
                .Where(u => u.Id != userId)
                .ToList();

            return View();
        }

        // JSON endpoint for unread count (used by layout JS)
        [HttpGet]
        public IActionResult UnreadCount()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var count = _context.Receivers.Count(r => r.ReceiverId == userId && !r.IsRead);
            return Json(new { count });
        }

        // JSON endpoint for getting conversation messages (AJAX polling/refresh)
        [HttpGet]
        public IActionResult GetMessages(int userId)
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var messages = _context.Messages
                .Include(m => m.Receivers)
                .Where(m =>
                    (m.SenderId == currentUserId && m.Receivers.Any(r => r.ReceiverId == userId)) ||
                    (m.SenderId == userId && m.Receivers.Any(r => r.ReceiverId == currentUserId))
                )
                .OrderBy(m => m.CreatedAt)
                .Select(m => new
                {
                    m.Id,
                    m.Content,
                    m.SenderId,
                    m.CreatedAt,
                    IsSentByMe = m.SenderId == currentUserId
                })
                .ToList();

            return Json(messages);
        }
    }
}