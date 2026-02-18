using System;
using Models;

namespace WebApplication1.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "message", "post", "like", etc.
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Optional: Link to related entity
        public int? RelatedEntityId { get; set; }
    }
}
