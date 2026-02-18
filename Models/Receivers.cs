using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace WebApplication1.Models
{
    public class Receivers
    {
        public int MessageId { get; set; }
        public Message Message { get; set; } = null!;

        public int ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;
        public bool IsRead { get; set; } = false;
    }
}