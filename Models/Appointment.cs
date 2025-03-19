using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Appointment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public User Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; } 
        

    }
}
