using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace app.Domain {
    public partial class DatabaseModel : DbContext {
        public DbSet<User> User { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Notification> Notification { get; set; }

        public string DbPath { get; }

        public DatabaseModel() : base() {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "main.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) {
            options.UseSqlite($"Data Source={DbPath}");
         }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>()
                .Property(e => e.IPs)
                .HasConversion(
                    v => string.Join(',', v),
                    v => new List<string>(v.Split(',', StringSplitOptions.RemoveEmptyEntries)),
                    new ValueComparer<List<string>>(
                        (a, b) => a.SequenceEqual(b),
                        a => a.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        a => a.ToList())
                );
        }
    }
}