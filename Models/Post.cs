using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Post
    {
        public int Id{get;set;}
        public string Title{get;set;}=string.Empty;
        public string Content{get;set;}=string.Empty;

        public DateTime CreatedAt{get;set;}=DateTime.UtcNow;

        public int UserId{get;set;}
        public User User{get;set;}=null!;

        List<PostCategory> PostCategories{get;set;}=new ();
    }
}