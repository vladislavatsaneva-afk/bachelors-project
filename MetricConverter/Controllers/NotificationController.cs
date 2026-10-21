using MetricConverter.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MetricConverter.Controllers
{
    public class NotificationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        public NotificationController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> UserNotis()
        {
            var userId = _userManager.GetUserId(User);
            var notis = await _context.UserNotifications
                .Where(n => n.UserId == userId)
                .Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notis);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            var noti = await _context.UserNotifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (noti == null) return NotFound();
            noti.IsRead = true;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
