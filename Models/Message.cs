using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace WebApplication1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public List<Receivers> Receivers { get; set; } = new ();
    }
}