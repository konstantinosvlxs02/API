using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using WebApplication1.Models;

namespace DataBase
{
    public class Database:DbContext
    {
        public Database(DbContextOptions<Database> options):base(options)
        {

        }
        public DbSet<User> Users{get;set;}=null!;

        public DbSet<Post> Posts{get;set;}=null!;
        public DbSet<Category> Categories{get;set;}=null!;
        public DbSet<PostCategory> PostCategories{get;set;}=null!;
        public DbSet<Message> Messages{get;set;}=null!;
        public DbSet<Receivers> Receivers{get;set;}=null!;
        public DbSet<Contact> Contact{get;set;}=null!;
        public DbSet<Notification> Notifications{get;set;}=null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Primary Key για το Posing Category
            modelBuilder.Entity<PostCategory>()
                .HasKey(pc => new { pc.PostId, pc.CategoryId });

            //Primary Key για το Receivers
            modelBuilder.Entity<Receivers>()
                .HasKey(r => new { r.MessageId, r.ReceiverId });
        }
    }
}