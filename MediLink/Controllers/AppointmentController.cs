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

        // ✅ STEP 6C – Show booking page
        [HttpGet]
        public IActionResult Book(int? hospitalId)
        {
            ViewBag.Hospitals = new SelectList(
                _context.Hospitals.ToList(),
                "Id",
                "Name",
                hospitalId
            );

            ViewBag.DoctorId = 1; // temporary default

            return View(new Appointment());
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Hospitals = new SelectList(
                    _context.Hospitals.ToList(),
                    "Id",
                    "Name",
                    appointment.HospitalId
                );

                ViewBag.DoctorId = appointment.DoctorId;
                return View(appointment);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            appointment.PatientId = user.Id;
            appointment.Status = "Booked";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyAppointments));
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
