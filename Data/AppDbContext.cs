
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebApplication1.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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
        // تهيئة علاقة PasswordResetToken مع User
        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(prt => prt.User)
            .WithMany()
            .HasForeignKey(prt => prt.UserId)
            .OnDelete(DeleteBehavior.Cascade); // حذف الرموز إذا حُذف المستخدم

    }
    public DbSet<User> Users { get; set; } // جدول المستخدمين
    public DbSet<EmailVerification> EmailVerifications { get; set; } // جدول أكواد التحقق
    public DbSet<Appointment> Appointments { set; get; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
}
