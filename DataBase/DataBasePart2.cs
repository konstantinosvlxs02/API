using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using part2_exersice.Model;

namespace part2_exersice.DataBase
{
    public class DataBasePart2:DbContext
    {
        public DataBasePart2(DbContextOptions<DataBasePart2> options):base(options)
        {

        }
        public DbSet<User> Users{get;set;}=null!;

        public DbSet<Todo> Todos{get;set;}=null!;
        public DbSet<TodoListItem> TodoListItems{get;set;}=null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cascade delete: Όταν διαγράφεται ένα Todo, διαγράφονται και τα TodoListItems του
            modelBuilder.Entity<Todo>()
                .HasMany(t => t.Tags)
                .WithOne(ti => ti.Todo)
                .HasForeignKey(ti => ti.TodoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}