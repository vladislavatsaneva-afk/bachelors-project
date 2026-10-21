using Microsoft.AspNetCore.Mvc;
using MetricConverter.Models;
using MetricConverter.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MetricConverter.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RegisterController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                ModelState.AddModelError("", $"Потребител с име '{model.Username} ВЕЧЕ съществува.");
                return View(model);
            }
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                ModelState.AddModelError("", $"Имейлът '{model.Email}' ВЕЧЕ е използван.");
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Converter", "Conversion");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}

