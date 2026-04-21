using GameLibraryDomain.Model;
using GameLibraryInfrastructure;
using GameLibraryInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GameLibraryInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GameLibraryDbContext _context;

        public HomeController(ILogger<HomeController> logger, GameLibraryDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var recentGames = await _context.Games
                .OrderByDescending(g => g.Id)
                .Take(6)
                .ToListAsync();

            return View(recentGames);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}