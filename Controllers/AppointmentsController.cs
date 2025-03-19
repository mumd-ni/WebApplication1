using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context) => _context = context;

        [HttpGet("search-doctors")]
        public async Task<IActionResult> SearchDoctors(string name)
        {
            var doctors = await _context.Users
                .Where(u => u.Role == "Doctor" && u.UserName.Contains(name))
                .ToListAsync();
            return Ok(doctors);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Unable to retrieve user ID from token.");
            return userId;
        }
         [Authorize(Roles = "Patient")]
        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           // var patientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var doctor = await _context.Users.FindAsync(request.DoctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            var appointment = new Appointment
            {
               // PatientId = patientId,
                PatientId = GetCurrentUserId(),
                DoctorId = request.DoctorId,
                AppointmentDate = request.AppointmentDate,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment booked successfully", appointment });
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => new
                {
                    a.Id,
                    PatientName = a.Patient.UserName,
                    a.AppointmentDate,
                    a.Status
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [Authorize(Roles = "Doctor")]
        [HttpPut("update-status/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId, [FromBody] string status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return NotFound("Appointment not found");

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment status updated", appointment });
        }


    }
}
