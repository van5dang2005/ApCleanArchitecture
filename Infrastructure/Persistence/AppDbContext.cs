using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Email).IsUnique();
                e.HasIndex(x => x.Username).IsUnique();
                e.Property(x => x.Email).IsRequired().HasMaxLength(256);
                e.Property(x => x.Username).IsRequired().HasMaxLength(100);
                e.Property(x => x.Role).HasDefaultValue("User");
            });
        }
    }
}
