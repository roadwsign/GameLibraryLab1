using Microsoft.AspNetCore.Mvc;

namespace GameLibraryInfrastructure.Controllers
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
