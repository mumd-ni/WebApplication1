// Controllers/FeedbackController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Dtos;
using WebApplication1.Models;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Patient")] // يقتصر على المرضى فقط
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FeedbackController(ApplicationDbContext context)
    {
        _context = context;
    }

    // دالة مساعدة لاستخراج معرف المستخدم من التوكن
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Unable to retrieve user ID from token.");
        }
        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> AddFeedback([FromBody] FeedbackDto dto)
    {
        // التحقق من وجود الطبيب
        var doctor = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == dto.DoctorId && u.Role == "Doctor");
        if (doctor == null)
            return NotFound("Doctor not found");

        // التحقق من أن المريض قد حجز موعدًا مع الطبيب مسبقًا (اختياري)
        var hasAppointment = await _context.Appointments
            .AnyAsync(a => a.PatientId == GetCurrentUserId() && a.DoctorId == dto.DoctorId);
        if (!hasAppointment)
            return BadRequest("You can only provide feedback for doctors you have booked with.");

        // إنشاء التعليق
        var feedback = new Feedback
        {
            PatientId = GetCurrentUserId(),
            DoctorId = dto.DoctorId,
            Comment = dto.Comment,
            Rating = dto.Rating
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Feedback added successfully" });
    }
    [HttpGet("doctor/{doctorId}")]
    public async Task<IActionResult> GetFeedbacksForDoctor(int doctorId)
    {
        var feedbacks = await _context.Feedbacks
            .Where(f => f.DoctorId == doctorId)
            .Include(f => f.Patient) // لعرض اسم المريض
            .Select(f => new
            {
                f.Id,
                PatientName = f.User.UserName,
                f.Comment,
                f.Rating,
                f.CreatedAt
            })
            .ToListAsync();

        if (!feedbacks.Any())
            return NotFound("No feedback found for this doctor");

        return Ok(feedbacks);
    }
}