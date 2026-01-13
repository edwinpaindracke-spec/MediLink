using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MediLink.Data;
using MediLink.Models;

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

        // ✅ STEP 6C – Show booking page
        [HttpGet]
        public IActionResult Book(int doctorId)
        {
            ViewBag.DoctorId = doctorId;
            return View();
        }

        // ✅ STEP 6D – Save appointment + send SMS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return View(appointment);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            appointment.PatientId = user.Id;
            appointment.Status = "Booked";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(); // ✅ SAVE FIRST

            // ✅ SEND SMS
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                _smsService.SendSms(
                    user.PhoneNumber,
                    $"Your appointment is confirmed for {appointment.AppointmentDate:dd MMM yyyy HH:mm}"
                );
            }

            return RedirectToAction("MyAppointments");
        }

        // ✅ View logged-in patient's appointments
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
