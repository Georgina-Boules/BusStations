using DB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    /* public class DBContext : DbContext
     {
         public DBContext(DbContextOptions<DBContext> options)
             : base(options)
         {
         }
         public DbSet<Bus> Buses { get; set; }
         public DbSet<User> Users { get; set; }

         // Optional: You can override OnModelCreating to configure model properties, relationships, etc.
         protected override void OnModelCreating(ModelBuilder modelBuilder)
         {
             // Configure User entity


             base.OnModelCreating(modelBuilder);
         }
     }
    */
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=LAPTOP-N7ONSEJR;Database=BusStations;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;",
                    b => b.MigrationsAssembly("WebAPI")); // Set to your main project assembly
            }
        }
    }

}
