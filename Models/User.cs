
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; } // تخزين كلمة المرور بشكل مشفر

    [Required]
    public string Role { get; set; } // "Patient" أو "Doctor"

    public bool IsEmailVerified { get; set; } = false; // حالة التحقق من البريد
    public List<Appointment>PatientAppointments { set; get; }
    public List<Appointment> DoctorAppointments { set; get; }
    public List<Feedback> DoctorFeedbacks { set; get; }
    public List<Feedback> PatientFeedbacks { set; get; }
}
