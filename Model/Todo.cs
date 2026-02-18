using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace part2_exersice.Model
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
        public List<TodoListItem> Tags { get; set; } = new ();
    }
}