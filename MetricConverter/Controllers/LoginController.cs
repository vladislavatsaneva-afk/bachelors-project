using MetricConverter.Data;
using MetricConverter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MetricConverter.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly EmailSender _emailSender;
        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context, EmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Models.LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                
                if (!string.IsNullOrWhiteSpace(model.NotiMessage))
                {
                    var not = new NotificationModel
                    {
                        UserId = user.Id,
                        Message = model.NotiMessage,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserNotifications.Add(not);
                    await _context.SaveChangesAsync();
                }

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Admin", "Admin");
                }
                else
                {
                    return RedirectToAction("Converter", "Conversion");
                }
            }

            ModelState.AddModelError("", "НЕВАЛИДНО потребителско име или парола.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPass()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPass(ForgotPassModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Няма потребител с такъв имейл адрес.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPass", "Login",
                new { token, email = model.Email }, protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                    user.Email,
                    "Нулиране на парола",
                    $"Моля, кликнете тук, за да смените паролата си: <a href='{callbackUrl}'>Смени парола</a>"
                );

            ViewBag.Message = "Инструкции за нулиране на паролата са изпратени на Вашия имейл."; 
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public IActionResult ResetPass(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("Login", "Login");

            var model = new ResetPassModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPass(ResetPassModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("Login", "Login");
            
            var email = model.Email?.Trim().ToLower();
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["Message"] = "Паролата беше успешно сменена.";
                return RedirectToAction("Login", "Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Password", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessageToAdmin(Models.LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.NotiMessage))
            {
                ModelState.AddModelError("", "Попълнете потребителско име и съобщение.");
                return View("Login", model);
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Потребителят не е намерен.");
                return View("Login", model);
            }
            var noti = new NotificationModel
            {
                UserId = user.Id,
                Message = model.NotiMessage,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserNotifications.Add(noti);
            await _context.SaveChangesAsync();

            TempData["MessageSent"] = "Съобщението е изпратено до администратора.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Login");
        }

    }
}
