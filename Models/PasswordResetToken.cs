using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } // علاقة مع المستخدم

        [Required]
        [StringLength(6)] // رمز مكون من 6 أرقام
        public string Token { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
    }
}
