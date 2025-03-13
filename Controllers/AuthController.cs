using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using WebApplication1.Services;
using NETCore.MailKit.Core;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Dtos;
using WebApplication1.Services;
using WebApplication1.Models;
using IEmailService = NETCore.MailKit.Core.IEmailService;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public AuthController(AppDbContext context, IEmailService emailService, ITokenService tokenService)
    {
        _context = context;
        _emailService = emailService;
        _tokenService = tokenService;
    }
    //public AuthController(AppDbContext context)
    //{
    //   _context = context;
    //}

    // تسجيل مستخدم جديد
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (_context.Users.Any(u => u.Email == request.Email))
            return BadRequest(new { message = "البريد الإلكتروني مسجل بالفعل" });

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = request.Role,
            IsEmailVerified = true
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { message = "تم تسجيل المستخدم بنجاح" });
    }

    // تسجيل الدخول
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "بيانات الدخول غير صحيحة" });

        return Ok(new { message = "تم تسجيل الدخول بنجاح", user = new { user.Id, user.Name, user.Email, user.Role } });
    }
    private string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString(); // رمز مكون من 6 أرقام
    }    
    // إرسال كود إعادة تعيين كلمة المرور} 
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return NotFound("User with this email does not exist");

        // توليد رمز التحقق
        var resetCode = GenerateVerificationCode();
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = resetCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15) // صلاحية 15 دقيقة
        };

        // حفظ الرمز في قاعدة البيانات
        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        // إرسال الرمز عبر البريد الإلكتروني
        await _emailService.SendAsync(
            dto.Email,
            "Reset Your Password",
            $"Your password reset code is: {resetCode}. It expires in 15 minutes."
        );

        return Ok(new { Message = "Reset code sent to your email" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return NotFound("User with this email does not exist");

        // التحقق من الرمز
        var resetToken = await _context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Token == dto.Token);
        if (resetToken == null)
            return BadRequest("Invalid reset code");
        if (resetToken.ExpiresAt < DateTime.UtcNow)
            return BadRequest("Reset code has expired");

        // تحديث كلمة المرور
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        _context.Users.Update(user);

        // حذف الرمز بعد الاستخدام
        _context.PasswordResetTokens.Remove(resetToken);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Password reset successfully" });

    }
}