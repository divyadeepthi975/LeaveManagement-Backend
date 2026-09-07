using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
