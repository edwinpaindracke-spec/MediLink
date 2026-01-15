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

        // ==============================
        // GET: Book Appointment
        // ==============================
        [HttpGet]
        public IActionResult Book()
        {
            ViewBag.Hospitals = new SelectList(
                _context.Hospitals.ToList(),
                "Id",
                "Name"
            );

            return View(new Appointment());
        }

        // ==============================
        // POST: Confirm Appointment
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Hospitals = new SelectList(
                    _context.Hospitals.ToList(),
                    "Id",
                    "Name"
                );

                return View(appointment);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            appointment.PatientId = user.Id;
            appointment.Status = "Pending";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Send SMS
            var hospital = await _context.Hospitals
                .FirstOrDefaultAsync(h => h.Id == appointment.HospitalId);

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var message =
 $@"MediLink Appointment Received 📋
Hospital: {hospital?.Name}
Date & Time: {appointment.AppointmentDateTime:dd MMM yyyy, hh:mm tt}

Status: Pending confirmation";

                try
                {
                    await _smsService.SendSmsAsync(user.PhoneNumber, message);
                }
                catch { }
            }

            return RedirectToAction(nameof(Confirmation), new { id = appointment.Id });
        }

        // ==============================
        // GET: Confirmation Page
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

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
