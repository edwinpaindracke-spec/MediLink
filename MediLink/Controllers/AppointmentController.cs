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
            // 🔥 Ignore Step-2 fields
            ModelState.Remove("DoctorId");
            ModelState.Remove("AppointmentDateTime");
            ModelState.Remove("HospitalName");

            if (!ModelState.IsValid)
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Id == model.HospitalId);

                model.HospitalName = hospital?.Name;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // ✅ STEP 1: SAVE BASIC INFO ONLY
            var appointment = new Appointment
            {
                HospitalId = model.HospitalId,
                PatientName = model.PatientName,
                Location = model.Location,
                ProblemDescription = model.ProblemDescription,

                            // ⛔ not yet

                PatientId = user.Id,
                Status = "Draft",           // 👈 VERY IMPORTANT
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // ✅ REDIRECT TO SUBMITTED PAGE
            return RedirectToAction("Submitted", new { id = appointment.Id });
        }


        [HttpGet]
        public async Task<IActionResult> Submitted(int id)
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
        // POST: Schedule Appointment
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(Appointment model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a =>
                    a.Id == model.Id && a.PatientId == user.Id);

            if (appointment == null)
                return NotFound();

            appointment.DoctorId = model.DoctorId;
            appointment.AppointmentDateTime = model.AppointmentDateTime;
            appointment.Status = "Confirmed";

            await _context.SaveChangesAsync();

            // ✅ SEND SMS AFTER FINAL CONFIRMATION
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.Id == appointment.HospitalId);

                var message =
                    "✔ MediLink Appointment Confirmed\n" +
                    $"Hospital: {hospital?.Name}\n" +
                    $"Date & Time: {appointment.AppointmentDateTime:dd MMM yyyy, hh:mm tt}\n\n" +
                    "Thank you for using MediLink.";

                try
                {
                    await _smsService.SendSmsAsync(user.PhoneNumber, message);
                }
                catch { }
            }

            return RedirectToAction("MyAppointments");
        }


        // ==============================
        // GET: Schedule Appointment
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Schedule(int id)
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

            // Doctors for this hospital
            ViewBag.Doctors = await _context.Doctors
                .Where(d => d.HospitalId == appointment.HospitalId)
                .ToListAsync();

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
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(appointments);
        }
    }
}
