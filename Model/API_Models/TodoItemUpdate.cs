using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace part2_exersice.Model.API_Models
{
    public class TodoItemUpdate
    {
         public string Content { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
    }
}