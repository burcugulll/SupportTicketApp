using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Enums;
using SupportTicketApp.Models;
using SupportTicketApp.ViewModels;
using System.Security.Claims;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SupportTicketApp.Context;


namespace SupportTicketApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SupportTicketDbContext _context;
        //private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SupportTicketDbContext context ) //SignInManager<IdentityUser> signInManager
        {
            _context = context;
            //_signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            /*if (_signInManager.IsSignedIn(User)) // Kullanıcı giriş yapmış mı?
            {
                return RedirectToAction("Index", "Home"); // Anasayfaya yönlendir
            }*/
            return View(); // Giriş yapmamışsa Login sayfasını göster
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserTab user;
                if (model.UsernameOrEmail.Contains("@"))
                {
                    user = _context.UserTabs.SingleOrDefault(u => u.Email == model.UsernameOrEmail);
                }
                else
                {
                    user = _context.UserTabs.SingleOrDefault(u => u.UserName == model.UsernameOrEmail);
                }

            if (user != null)
                {
                    if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.Now)
                    {
                        ViewBag.Message = "Çok sayıda hatalı giriş denemesi yaptınız. Lütfen 1 dakika sonra tekrar deneyin.";
                        return View();
                    }

                var hashedPassword = user.HashPassword(model.Password);
                if (user.Password == hashedPassword)
                    {
                        user.LoginAttempts = 0;
                        user.LockoutEndTime = null;

                        var userLog = new UserLogTab
                        {
                            UserName = user.UserName,
                            LogTime = DateTime.Now,
                            UserId = user.UserId,
                            IPAdress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            Log = "Başarılı giriş"
                        };
                        _context.UserLogTabs.Add(userLog);
                        _context.SaveChanges();
                        var base64ProfilePhoto = user.ProfilePhoto != null ? Convert.ToBase64String(user.ProfilePhoto) : null;

                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.UserType.ToString()) ,


                    };
                        // Profil fotoğrafını Claim'e ekle
                        if (base64ProfilePhoto != null)
                        {
                            claims.Add(new Claim("ProfilePhoto", base64ProfilePhoto));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, "login");

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                        if (user.UserType == UserType.Yonetici)
                        {

                            return RedirectToAction("Index", "Admin");
                        }
                        else if (user.UserType == UserType.Calisan)
                        {
                            return RedirectToAction("AssignedTickets", "Employee");

                        }
                        else
                        {
                            return RedirectToAction("Index", "User");
                        }
                    }
                    else
                    {
                        user.LoginAttempts++;
                        var userLog = new UserLogTab
                        {
                            UserName = user.UserName,
                            LogTime = DateTime.Now,
                            UserId = user.UserId,
                            IPAdress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            Log = "Başarısız giriş"
                        };
                        _context.UserLogTabs.Add(userLog);
                        _context.SaveChanges();
                        if (user.LoginAttempts >= 3)
                        {
                            user.LockoutEndTime = DateTime.Now.AddMinutes(1);
                            ViewBag.Message = "3 kez yanlış şifre girdiniz. 1 dakika boyunca giriş yapamazsınız.";
                        }
                        else
                        {
                            ViewBag.Message = $"Geçersiz kullanıcı adı veya şifre! {3 - user.LoginAttempts} hakkınız kaldı.";
                        }
                        _context.SaveChanges();
                    }
                }
                else
                {
                    ViewBag.Message = "Böyle bir kullanıcı veritabanında kayıtlı değil.";
                }

                
            }
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {

            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            /*if (_signInManager.IsSignedIn(User)) // Kullanıcı giriş yapmış mı?
            {
                return RedirectToAction("Index", "Home"); // Anasayfaya yönlendir
            }*/
            return View();
        }

        [HttpGet]
        public IActionResult Test()
        {
            var result = new { message = "Test" };  // JSON dönecek veriyi oluşturuyoruz
            return new JsonResult(result);  // JsonResult ile veriyi JSON formatında döndürüyoruz
        }


        [HttpGet]
        public async Task<IActionResult> CreateAdminUser()
        {
            try
            {
                Console.WriteLine("1 - Salt oluşturuluyor");
      
                string salt = SupportTicketApp.Models.UserTab.GenerateSalt();
                Console.WriteLine("2 - Şifre hashleniyor");

                string hashedPassword = new UserTab { Salt = salt }.HashPassword("admin123"); // Şifre: admin123
                if (_context.UserTabs == null)
                {
                    ViewBag.Message = "UserTabs tablosuna erişilemiyor.";
                    return View();
                }
                string adminEmail = "admin@example.com";
                var existingAdmin = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == "admin");
                if (existingAdmin != null)
                {
                    return new JsonResult(new { code = 404, message = "Admin kullanıcı zaten mevcut."});  // JsonResult ile veriyi JSON formatında döndürüyoruz
                    ViewBag.Message = "Admin kullanıcı zaten mevcut.";

                    return View();
                }
                var adminUser = new UserTab
                {
                    Email = adminEmail,
                    UserName = "admin",
                    Password = hashedPassword,
                    Salt = salt,
                    Name = "Admin User",
                    UserType = UserType.Yonetici,
                    Status = true,
                    LoginAttempts = 0,
                    LockoutEndTime = null,
                    ProfilePhoto = new byte[0], 
                    CreatedDate = DateTime.Now, 
                    ModifiedDate = null, 
                    DeletedDate = null 
                };

                _context.UserTabs.Add(adminUser);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    ViewBag.Message = "Yönetici kullanıcı başarıyla oluşturuldu!";
                }
                else
                {
                    ViewBag.Message = "Yönetici kullanıcı kaydedilemedi. Veritabanında bir sorun olabilir.";
                }

                var jsonresult = new { code=200, message = "Admin Kullanıcısı Başarıyla Oluşturuldu." };  // JSON dönecek veriyi oluşturuyoruz
                return new JsonResult(jsonresult);  // JsonResult ile veriyi JSON formatında döndürüyoruz


            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, message = "Admin Kullanıcısı Oluşturulamadı",error = ex.ToString() });  // JsonResult ile veriyi JSON formatında döndürüyoruz
            }

        }
        
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.UserTabs.Any(u => u.UserName == model.UserName || u.Email == model.Email))
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı veya e-posta zaten alınmış.");
                    return View(model);
                }

                var user = new UserTab
                {
                    Email = model.Email,  
                    UserName = model.UserName,
                    Name = model.Name,
                    UserType = UserType.SonKullanici,
                    Status = true,
                    ProfilePhoto = new byte[0] 

                };

                string salt = SupportTicketApp.Models.UserTab.GenerateSalt();
                user.Salt = salt;
                user.Password = user.HashPassword(model.Password);

                _context.UserTabs.Add(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Başarıyla kaydoldunuz.Lütfen giriş yapın.";

                return RedirectToAction("Login");
            }

            return View(model);
        }


    }
}
















            //public IActionResult Index()
            //{
            //    return View();
            //}
        

