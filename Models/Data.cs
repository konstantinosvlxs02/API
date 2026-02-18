using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Data
    {
        public string content { get; set; } = string.Empty;
        public List<int> receiverIds { get; set; } = new ();
    }
}