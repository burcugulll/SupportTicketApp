using Microsoft.AspNetCore.Mvc;
using SupportTicketApp.Models;
using System.Diagnostics;
using SupportTicketApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SupportTicketApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SupportTicketDbContext _context;

        
        public HomeController(ILogger<HomeController> logger, SupportTicketDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var userName = User.Identity.Name;
            var user = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName); // SingleOrDefault kullan�m�
            if (user == null)
            {
                return NotFound("Kullan�c� bulunamad�.");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string Name, string Email, string Password, IFormFile ProfilePhoto)
        {
            var userName = User.Identity.Name;
            var user = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName); // SingleOrDefault kullan�m�
            if (user == null)
            {
                return NotFound("Kullan�c� bulunamad�.");
            }
            bool changesMade = false;

            if (user.Name != Name)
            {
                user.Name = Name;
                changesMade = true;
            }

            if (user.Email != Email)
            {
                user.Email = Email;
                changesMade = true;
            }

            if (!string.IsNullOrEmpty(Password))
            {
                string salt = UserTab.GenerateSalt();
                user.Salt = salt;
                user.Password = user.HashPassword(Password);
                changesMade = true;

            }

            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ProfilePhoto.CopyToAsync(memoryStream);
                    user.ProfilePhoto = memoryStream.ToArray();
                    changesMade = true;

                }
            }
            //else if (user.ProfilePhoto == null || user.ProfilePhoto.Length == 0)
            //{
            //    // E�er kullan�c� hi� foto�raf y�klememi�se varsay�lan avatar� ayarla
            //    var defaultAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/default.png");
            //    if (System.IO.File.Exists(defaultAvatarPath))
            //    {
            //        user.ProfilePhoto = await System.IO.File.ReadAllBytesAsync(defaultAvatarPath);
            //    }
            //}

            if (changesMade)
            {
                _context.UserTabs.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profil ba�ar�yla g�ncellendi.";
            }
            else
            {
                TempData["InfoMessage"] = "Profilinizde bir de�i�iklik yap�lmad�.";
            }
            
            return RedirectToAction("Settings");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
