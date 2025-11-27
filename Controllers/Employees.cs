using Microsoft.AspNetCore.Mvc;

namespace PublishingHouseApp.Controllers
{
    public class Employees : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
