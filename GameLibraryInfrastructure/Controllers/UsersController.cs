using GameLibraryDomain.Model;
using GameLibraryInfrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryInfrastructure.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                {
                    TempData["ErrorMessage"] = "Не можна змінити права Суперадміністратора!";
                    return RedirectToAction(nameof(Index));
                }

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}