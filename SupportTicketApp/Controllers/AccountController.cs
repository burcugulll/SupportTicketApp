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


namespace SupportTicketApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SupportTicketDbContext _context;

        public AccountController(SupportTicketDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            //if (ModelState.IsValid)
            //{
                var user = _context.UserTabs.SingleOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.Now)
                    {
                        ViewBag.Message = "Çok sayıda hatalı giriş denemesi yaptınız. Lütfen 1 dakika sonra tekrar deneyin.";
                        return View();
                    }

                    var hashedPassword = user.HashPassword(password);
                    if (user.Password == hashedPassword)
                    {
                        user.LoginAttempts = 0;
                        user.LockoutEndTime = null;

                        var userLog = new UserLogTab
                        {
                            UserName = username,
                            LogTime = DateTime.Now,
                            IPAdress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            Log = "Başarılı giriş"
                        };
                        _context.UserLogTabs.Add(userLog);
                        _context.SaveChanges();

                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.UserType.ToString()) 
                    };

                        var claimsIdentity = new ClaimsIdentity(claims, "login");

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                        if (user.UserType == UserType.Yonetici)
                        {

                            return RedirectToAction("Index", "Admin");
                        }
                        else if (user.UserType == UserType.Calisan)
                        {
                            return RedirectToAction("Index", "Employee");

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
                            UserName = username,
                            LogTime = DateTime.Now,
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

                return View();
            }
        //    return View();

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {

            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return View();
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
                var existingAdmin = await _context.UserTabs.SingleOrDefaultAsync(u => u.UserName == "admin");
                if (existingAdmin != null)
                {
                    ViewBag.Message = "Admin kullanıcı zaten mevcut.";
                    return View();
                }
                var adminUser = new UserTab
                {
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
                return View(); 

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Yönetici kullanıcı oluşturulurken bir hata oluştu: " + ex.Message;
                return View();

            }

        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.UserTabs.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten alınmış.");
                    return View(model);
                }

                var user = new UserTab
                {
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
        

