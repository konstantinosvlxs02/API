using System;
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

namespace WebApplication1.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly Database _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(Database context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Notification
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var notifications = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }

        // POST: Notification/MarkRead/5
        [HttpPost]
        public IActionResult MarkRead(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                _context.SaveChanges();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true });

            return RedirectToAction("Index");
        }

        // POST: Notification/MarkAllRead
        [HttpPost]
        public IActionResult MarkAllRead()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var notifications = _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            foreach (var n in notifications)
                n.IsRead = true;

            _context.SaveChanges();

            TempData["Success"] = $"{notifications.Count} ειδοποιήσεις σημειώθηκαν ως αναγνωσμένες.";
            return RedirectToAction("Index");
        }

        // POST: Notification/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // JSON endpoint for unread count (used by layout JS)
        [HttpGet]
        public IActionResult UnreadCount()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var count = _context.Notifications.Count(n => n.UserId == userId && !n.IsRead);
            return Json(new { count });
        }
    }
}
