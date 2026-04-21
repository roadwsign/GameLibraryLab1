using DocumentFormat.OpenXml.InkML;
using GameLibraryDomain.Model;
using GameLibraryInfrastructure.Models;
using GameLibraryInfrastructure.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GameLibraryInfrastructure.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GameLibraryDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, GameLibraryDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Username,
                    Createdat = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Games");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        return RedirectToAction("Index", "Games");
                    }
                }
                ModelState.AddModelError("", "Неправильний логін та (або) пароль");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var history = await _context.Statushistories
            .Include(sh => sh.Newstatus)
            .Include(sh => sh.Oldstatus)
            .Include(sh => sh.Userlibrary)
                .ThenInclude(ul => ul.Game)
            .Where(sh => sh.Userlibrary.Userid == user.Id)
            .OrderByDescending(sh => sh.Changedate)
            .ToListAsync();

            var model = new ProfileViewModel
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                History = history
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsername(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (setUserNameResult.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["StatusMessage"] = "Нікнейм успішно змінено!";
                    return RedirectToAction(nameof(Profile));
                }
                foreach (var error in setUserNameResult.Errors)
                {
                    ModelState.AddModelError("UserName", error.Description);
                }
            }

            model.Email = user.Email ?? "";
            return View("Profile", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            ModelState.Remove("UserName");

            if (!string.IsNullOrWhiteSpace(model.OldPassword) && !string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (changePasswordResult.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["StatusMessage"] = "Пароль успішно змінено!";
                    return RedirectToAction(nameof(Profile));
                }
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("OldPassword", error.Description);
                }
            }

            model.UserName = user.UserName ?? "";
            model.Email = user.Email ?? "";
            return View("Profile", model);
        }
    }
}