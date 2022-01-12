using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace VMS.Models
{
    public partial class VMSDbContext : DbContext
    {
        public VMSDbContext()
        {
        }

        public VMSDbContext(DbContextOptions<VMSDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=VMSDb;Integrated Security=True");
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.CarRegistration).HasMaxLength(50);

                entity.Property(e => e.CompanyName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FullName).HasMaxLength(30);

                entity.Property(e => e.IsPhoto).HasColumnName("isPhoto");

                entity.Property(e => e.MeetingDescription).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(30);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.Description)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
