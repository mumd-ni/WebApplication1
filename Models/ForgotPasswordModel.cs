using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    
        public class ForgotPasswordModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }

