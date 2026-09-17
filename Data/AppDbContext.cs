using LeaveManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Leavetype> Leavetypes { get; set; }

        public DbSet<Leaverequest> Leaverequests { get; set; }

        public DbSet<Leavebalance> Leavebalances { get; set; }
        public DbSet<Loginusers> Loginuser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Leaverequest>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.employeeid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Leaverequest>()
                .HasOne(l => l.Leavetype)
                .WithMany()
                .HasForeignKey(l => l.leavetypeid)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Leavetype>()
                .HasIndex(l => l.leavetypename)
                .IsUnique();

            modelBuilder.Entity<Leavebalance>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.employeeid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Leavebalance>()
                .HasOne(l => l.Leavetype)
                .WithMany()
                .HasForeignKey(l => l.leavetypeid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Leavebalance>()
                .HasIndex(l => new
                {
                    l.employeeid,
                    l.leavetypeid
                })
                .IsUnique();

            modelBuilder.Entity<Loginusers>()
                .HasOne(l => l.employee)
                .WithOne()
                .HasForeignKey<Loginusers>(l => l.employeeid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loginusers>()
            .HasIndex(l => l.username)
            .IsUnique();
        }
    }
}