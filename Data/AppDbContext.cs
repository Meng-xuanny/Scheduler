using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scheduler.Models;

namespace Scheduler.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Job> Job { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ServiceRequest> ServiceRequest { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<ProviderNotification> ProviderNotification { get; set; }

        public DbSet<Provider> Provider { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.StartTime).HasColumnType("timestamp without time zone");
                entity.Property(e => e.EndTime).HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.Property(e => e.PreferredDate).HasColumnType("timestamp without time zone");
                entity.Property(e => e.RequestedAt).HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
            });


        }
    }
}
