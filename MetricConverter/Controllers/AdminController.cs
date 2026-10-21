using MetricConverter.Data;
using MetricConverter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricConverter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Admin()
        {
            var model = new AdminModel
            {
                Users = _userManager.Users.ToList(),
                ConversionHistoryEntries = _context.ConversionHistoryEntries
                    .Include(e => e.User)
                    .OrderByDescending(e => e.DateTime)
                    .ToList(),
                UserNotifications = _context.UserNotifications
                    .Include(n => n.User)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var noti = await _context.UserNotifications.FindAsync(id);
            if (noti != null)
            {
                noti.IsRead = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Admin", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNoti(int id)
        {
            var noti = await _context.UserNotifications.FindAsync(id);
            if (noti == null) return NotFound();

            _context.UserNotifications.Remove(noti);
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string username, string newU, string newPass, string newEmail)
        {
            if (string.IsNullOrEmpty(newU))
            {
                ModelState.AddModelError(nameof(newU), "Потребителското име НЕ може да е празно.");
                var User = await _userManager.FindByNameAsync(username);
                return View(User);
            }
            var userToUpdate = await _userManager.FindByNameAsync(username);
            if (userToUpdate == null)
                return NotFound();

            bool changes = false;
            string notiMessage = "";

            if (!string.Equals(userToUpdate.UserName, newU, StringComparison.OrdinalIgnoreCase))
            {
                userToUpdate.UserName = newU;
                var userNameResult = await _userManager.UpdateAsync(userToUpdate);
                if (!userNameResult.Succeeded)
                {
                    foreach (var error in userNameResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(userToUpdate);
                }
                changes = true;
                notiMessage += $"Потребителското име беше сменено на '{newU}'. ";
            }

            if (!string.IsNullOrWhiteSpace(newEmail) && !string.Equals(userToUpdate.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByEmailAsync(newEmail);
                if (existingUser != null && existingUser.Id != userToUpdate.Id)
                {
                    ModelState.AddModelError(nameof(newEmail), "Имейл адресът вече е зает от друг потребител.");
                    return View(userToUpdate);
                }

                userToUpdate.Email = newEmail;
                userToUpdate.NormalizedEmail = _userManager.NormalizeEmail(newEmail); 
                var emailResult = await _userManager.UpdateAsync(userToUpdate);
                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(userToUpdate);
                }
                changes = true;
                notiMessage += $"Имейлът беше обновен на '{newEmail}'. ";
            }

            if (!string.IsNullOrEmpty(newPass))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(userToUpdate);
                var passResult = await _userManager.ResetPasswordAsync(userToUpdate, token, newPass);

                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(userToUpdate);
                }
                changes = true;
                notiMessage += "Паролата беше обновена.";
            }

            if (changes)
            {
                var noti = new NotificationModel
                {
                    UserId = userToUpdate.Id,
                    Message = notiMessage.Trim(),
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.UserNotifications.Add(noti);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username)) 
                return BadRequest("Изисква се потребителско име.");
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) 
                return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Admin");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            var entry = await _context.ConversionHistoryEntries.FindAsync(id);
            if (entry == null)
                return NotFound();

            _context.ConversionHistoryEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return RedirectToAction("Admin");
        }
    }
}
