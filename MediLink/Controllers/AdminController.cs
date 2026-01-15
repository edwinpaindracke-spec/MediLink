using MediLink.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediLink.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Hospital)
                .Include(a => a.Patient)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Confirm(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = "Confirmed";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
