using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos
{
    public class AppointmentRequestDTO
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
    }
}
