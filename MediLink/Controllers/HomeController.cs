using MediLink.Data;
using MediLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace MediLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index(string searchTerm, string province, string city)
        {
            var query = _context.Hospitals.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(h => h.Name.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(province))
            {
                query = query.Where(h => h.Province == province);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(h => h.City == city);
            }

            var hospitals = await query.ToListAsync();

            // Pass current filters back to ViewData for keeping form inputs sticky
            ViewData["SearchTerm"] = searchTerm;
            ViewData["Province"] = province;
            ViewData["City"] = city;

            return View(hospitals);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
