
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApplication1.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
       
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>()
       .HasOne(a => a.Patient)
       .WithMany(p => p.PatientAppointments)
       .HasForeignKey(a => a.PatientId)
       .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.DoctorAppointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.NoAction);
        
            // علاقة One-to-Many بين User (كطبيب) و Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Doctor)
                .WithMany(u => u.DoctorFeedbacks)
                .HasForeignKey(f => f.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتتالي

            // علاقة One-to-Many بين User (كمريض) و Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Patient)
                .WithMany(u => u.PatientFeedbacks)
                .HasForeignKey(f => f.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتتالي

    }
    public DbSet<User> Users { get; set; } 
    public DbSet<Appointment> Appointments { set; get; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }


}
