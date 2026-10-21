using Microsoft.AspNetCore.Identity;
using MetricConverter.Data;
using MetricConverter.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MetricConverter.Controllers
{
    public class ConversionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ConversionController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ConversionModel
            {
                ConversionType = "Разстояние",
                FromUnit = "Километър",
                ToUnit = "Метър"
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ConversionModel model)
        {
            if (ModelState.IsValid)
            {
                model.Convert();
                ViewBag.Result = model.Result;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Converter()
        {
            var model = new ConversionModel(); 
            InitializeDefaultValues(model);
            await LoadConversionHistoryAsync(model); 
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Converter(ConversionModel model)
        {
            if (ModelState.IsValid)
            {
                model.Convert();
                await LogConversionHistoryAsync(model);
                await LoadConversionHistoryAsync(model);
                ViewBag.Result = model.Result;

                if (!string.IsNullOrEmpty(model.WarningMessage))
                {
                    ModelState.AddModelError(string.Empty, model.WarningMessage);
                }
            }
            return View(model);
        }

        private void InitializeDefaultValues(ConversionModel model)
        {
            model.ConversionType = "Разстояние";
            model.FromUnit = "Километър";
            model.ToUnit = "Метър";
        }

        private async Task LogConversionHistoryAsync(ConversionModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            var entry = new ConversionHistoryEntry
            {
                ConversionType = model.ConversionType,
                InputValue = model.Value,
                FromUnit = model.FromUnit,
                ToUnit = model.ToUnit,
                Result = model.Result,
                DateTime = DateTime.Now,
                UserID = user.Id
            };

            _context.ConversionHistoryEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
        private async Task LoadConversionHistoryAsync(ConversionModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                model.History = new List<ConversionHistoryEntry>();
                return;
            }

            model.History = await _context.ConversionHistoryEntries
                .Where(e => e.UserID == user.Id)
                .OrderByDescending(e => e.DateTime)
                .Take(20)
                .ToListAsync();
        }
        [HttpGet]
        public async Task<IActionResult> DownloadHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var history = await _context.ConversionHistoryEntries
                .Where(e => e.UserID == user.Id)
                .OrderByDescending(e => e.DateTime)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("ConversionHistory");
                worksheet.Cells[1, 1].Value = "Категория";
                worksheet.Cells[1, 2].Value = "Стойност";
                worksheet.Cells[1, 3].Value = "От";
                worksheet.Cells[1, 4].Value = "В";
                worksheet.Cells[1, 5].Value = "Резултат";
                worksheet.Cells[1, 6].Value = "Дата";

                for (int i = 0; i < history.Count; i++)
                {
                    var entry = history[i];
                    worksheet.Cells[i + 2, 1].Value = entry.ConversionType;
                    worksheet.Cells[i + 2, 2].Value = entry.InputValue;
                    worksheet.Cells[i + 2, 3].Value = entry.FromUnit;
                    worksheet.Cells[i + 2, 4].Value = entry.ToUnit;
                    worksheet.Cells[i + 2, 5].Value = entry.Result;
                    worksheet.Cells[i + 2, 6].Value = entry.DateTime.ToString("g");
                }
                await package.SaveAsync();
            }

            stream.Position = 0;
            string excelName = $"ConversionHistory-{DateTime.Now:ddMMyyyy}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteHistoryEntry(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var entryToRemove = await _context.ConversionHistoryEntries
                .FirstOrDefaultAsync(e => e.ID == id && e.UserID == user.Id);

            if (entryToRemove != null)
            {
                _context.ConversionHistoryEntries.Remove(entryToRemove);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Converter");
        }
    }
}
