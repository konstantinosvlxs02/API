using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace part2_exersice.Model
{
    public class User
    {
         public int Id{get;set;}

        public string Username{get;set;}=string.Empty;
        public string Email{get;set;}=string.Empty;

        public string Hash{get;set;}=string.Empty;
        public string Salt{get;set;}=string.Empty;

        public DateTime CreatedAt{get;set;}=DateTime.UtcNow;
    }
}