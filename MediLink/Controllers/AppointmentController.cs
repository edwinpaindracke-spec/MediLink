using MediLink.Data;
using MediLink.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediLink.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SmsService _smsService;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentController(
            ApplicationDbContext context,
            SmsService smsService,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _smsService = smsService;
            _userManager = userManager;
        }

        // ==============================
        // GET: Book Appointment
        // ==============================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Book(int hospitalId)
        {
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == hospitalId);

            if (hospital == null)
                return NotFound();

            var model = new AppointmentViewModel
            {
                HospitalId = hospital.Id,
                HospitalName = hospital.Name
            };

            return View(model);
        }

        // ==============================
        // POST: Book Appointment
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 🔴 IMPORTANT: re-load hospital name if validation fails
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Id == model.HospitalId);

                model.HospitalName = hospital?.Name;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var appointment = new Appointment
            {
                HospitalId = model.HospitalId,
                PatientName = model.PatientName,
                Location = model.Location,
                ProblemDescription = model.ProblemDescription,
                AppointmentDateTime = model.AppointmentDateTime,
                PatientId = user.Id,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // ==============================
            // Send SMS Notification
            // ==============================
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Id == model.HospitalId);

                var message =
$@"MediLink Appointment Submitted Successfully ✅
Hospital: {hospital?.Name}
Date & Time: {appointment.AppointmentDateTime:dd MMM yyyy, hh:mm tt}

You can view your appointment in your profile.";

                try
                {
                    await _smsService.SendSmsAsync(user.PhoneNumber, message);
                }
                catch
                {
                    // Log if needed, but don't block user
                }
            }

            TempData["Success"] = "Appointment submitted successfully!";
            return RedirectToAction(nameof(Confirmation), new { id = appointment.Id });
        }

        // ==============================
        // GET: Confirmation Page
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var appointment = await _context.Appointments
                .Include(a => a.Hospital)
                .FirstOrDefaultAsync(a =>
                    a.Id == id && a.PatientId == user.Id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // ==============================
        // GET: My Appointments
        // ==============================
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);

            var appointments = await _context.Appointments
                .Include(a => a.Hospital)
                .Where(a => a.PatientId == user.Id)
                .ToListAsync();

            return View(appointments);
        }
    }
}
