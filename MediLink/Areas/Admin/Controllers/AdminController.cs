using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLink.Areas.Admin.Controllers
{
    // Tell ASP.NET Core this controller belongs to the Admin area
    [Area("Admin")]

    // Only users in the "Admin" role can access any action here
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Admin dashboard homepage
        public IActionResult Index()
        {
            return View();
        }
    }
}
