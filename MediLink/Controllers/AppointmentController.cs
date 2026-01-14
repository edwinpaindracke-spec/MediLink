using MediLink.Data;
using MediLink.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // ✅ STEP 6C – Show booking page (GET Book)
        [HttpGet]
        public IActionResult Book(int? hospitalId)
        {
            var appointment = new Appointment
            {
                HospitalId = hospitalId ?? 0,
                DoctorId = 1 // temporary default, adjust as needed
            };

            ViewBag.Hospitals = new SelectList(
                _context.Hospitals.ToList(),
                "Id",
                "Name",
                appointment.HospitalId
            );

            return View(appointment);
        }

        // POST Book - confirm and save appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAppointment(Appointment appointment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            appointment.PatientId = user.Id;
            appointment.Status = "Booked";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Fetch hospital for SMS
            var hospital = await _context.Hospitals.FirstOrDefaultAsync(h => h.Id == appointment.HospitalId);

            var formattedDateTime = appointment.AppointmentDateTime.ToString("dd MMM yyyy, hh:mm tt");

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                string smsMessage =
        $@"MediLink Appointment Confirmed ✅
Hospital: {hospital?.Name}
Date & Time: {formattedDateTime}
Thank you for using MediLink.";

                try
                {
                    await _smsService.SendSmsAsync(user.PhoneNumber, smsMessage);
                }
                catch { /* ignore SMS failures */ }
            }

            return RedirectToAction(nameof(Confirmation), new { id = appointment.Id });
        }


        // Confirmation page showing appointment details
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var appointment = await _context.Appointments
                .Include(a => a.Hospital)
                .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == user.Id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // View logged-in patient's appointments
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);

            var appointments = await _context.Appointments
                .Where(a => a.PatientId == user.Id)
                .ToListAsync();

            return View(appointments);
        }
    }
}
