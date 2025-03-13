using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Appointment
    {
       
        
            public int Id { get; set; }

            [Required]
            public int PatientId { get; set; }
            public User Patient { get; set; }

            [Required]
            public int DoctorId { get; set; }
            public User Doctor { get; set; }

            [Required]
            public DateTime AppointmentDate { get; set; }

            public string Status { get; set; } = "Pending"; // يمكن أن يكون Confirmed, Cancelled, etc.
        

    }
}
