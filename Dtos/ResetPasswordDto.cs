namespace WebApplication1.Dtos
{
    public class ResetPasswordDto
    {
        // Dtos/ResetPasswordDto.cs
       
            public string Email { get; set; }
            public string Token { get; set; }
            public string NewPassword { get; set; }
        
    }
}
