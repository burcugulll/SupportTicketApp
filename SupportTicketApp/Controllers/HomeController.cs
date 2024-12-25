using Microsoft.AspNetCore.Mvc;
using SupportTicketApp.Models;
using System.Diagnostics;
using SupportTicketApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Context;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
            var user = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName); 
            if (user == null)
            {
                return NotFound("Kullanýcý bulunamadý.");
            }
            if (user.ProfilePhoto != null && user.ProfilePhoto.Length > 0)
            {
                ViewBag.ProfilePhotoBase64 = Convert.ToBase64String(user.ProfilePhoto);
            }
            else
            {
                ViewBag.ProfilePhotoBase64 = null;
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string UserName, string Name, string Email, string Password, IFormFile ProfilePhoto)
        {
            var userName = User.Identity.Name;
            var user = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName); 
            if (user == null)
            {
                return NotFound("Kullanýcý bulunamadý.");
            }
            bool changesMade = false;
            if (user.UserName != UserName)
            {
                var existingUser = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == UserName);
                if (existingUser != null)
                {
                    TempData["InfoMessage"] = "Bu kullanýcý adý zaten alýnmýþ.";
                    return RedirectToAction("Settings");
                }
                user.UserName = UserName;
                changesMade = true;
            }
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
            byte[] profilePhotoBytes = null;
            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ProfilePhoto.CopyToAsync(memoryStream);
                    profilePhotoBytes = memoryStream.ToArray();
                    user.ProfilePhoto = memoryStream.ToArray();
                    changesMade = true;

                }
            }

            if (changesMade)
            {
                _context.UserTabs.Update(user);
                await _context.SaveChangesAsync();


                TempData["SuccessMessage"] = "Profil baþarýyla güncellendi.";
                var base64ProfilePhoto = Convert.ToBase64String(user.ProfilePhoto);
                TempData["ProfilePhotoBase64"] = base64ProfilePhoto;
                if (profilePhotoBytes != null)
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    var profilePhotoClaim = identity.FindFirst("ProfilePhoto");

                    if (profilePhotoClaim != null)
                    {
                        identity.RemoveClaim(profilePhotoClaim);
                    }

                    identity.AddClaim(new Claim("ProfilePhoto", base64ProfilePhoto));

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(principal);
                }
            }
            else
            {
                TempData["InfoMessage"] = "Profilinizde bir deðiþiklik yapýlmadý.";
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
