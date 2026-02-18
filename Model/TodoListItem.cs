using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace part2_exersice.Model
{
    public class TodoListItem
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
        public int TodoId { get; set; }
        public Todo? Todo { get; set; }

    }
}