using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace WebApplication1.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User{get;set;}=null!;

        public int ContactUserId { get; set; }
        public User ContactUser { get; set; } = null!;
        DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}