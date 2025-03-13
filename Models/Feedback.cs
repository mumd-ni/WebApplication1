using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; } // علاقة مع المريض

        [Required]
        public int DoctorId { get; set; }
        public User Doctor { get; set; } // علاقة مع الطبيب

        [Required]
        [StringLength(500)]
        public string Comment { get; set; } // التعليق (حد أقصى 500 حرف)

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // التقييم من 1 إلى 5

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ الإنشاء
    }
}
