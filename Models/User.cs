
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

public class User : IdentityUser<int>
{

    [Required]
    public string Role { get; set; } // "Patient" أو "Doctor"

    public bool IsActive { get; internal set; }
    public List<Appointment>PatientAppointments { set; get; }
    public List<Appointment> DoctorAppointments { set; get; }
    public List<Feedback> DoctorFeedbacks { set; get; }
    public List<Feedback> PatientFeedbacks { set; get; }

    public List<RefreshToken>? RefreshTokens { get; set; }
}
