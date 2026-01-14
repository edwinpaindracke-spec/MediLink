using Microsoft.AspNetCore.Mvc;

namespace MediLink.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
