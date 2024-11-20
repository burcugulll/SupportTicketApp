using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "SonKullanici")]

    public class UserController : Controller
    {
        private readonly SupportTicketDbContext dbContext;

        public UserController(SupportTicketDbContext context)
        {
            dbContext = context;
        }


        public async Task<IActionResult> Index()
        {
            // Veritabanındaki TicketInfoTab kayıtlarını getiriyoruz.
            var tickets = await dbContext.TicketInfoTabs
                .Include(t => t.UserTab) // Kullanıcı bilgilerini çekmek için.
                .ToListAsync();

            // Razor sayfasına model olarak gönderiyoruz.
            return View(tickets);
        }


        //public async Task<IActionResult> Index()
        //{
        //    var userName = User.Identity.Name;
        //    var user = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName);

        //    if (user == null)
        //    {
        //        return NotFound("Kullanıcı bulunamadı.");
        //    }

        //    // Kullanıcıya ait biletleri getir
        //    var tickets = await dbContext.TicketInfoTabs
        //        .Where(t => t.UserId == user.UserId)
        //        .ToListAsync();

        //    return View(tickets);
        //}

        // Yeni bilet oluşturma
        [HttpGet]
        public IActionResult CreateTicket()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> CreateTicket(TicketInfoTab model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı al
                var userName = User.Identity.Name;
                var user = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                    return View(model);
                }
                //Console.WriteLine($"Title: {model.Title}, Description: {model.Description}, Urgency: {model.Urgency}");

                var newTicket = new TicketInfoTab
                {
                    Title = model.Title,
                    Description = model.Description,
                    Urgency = model.Urgency,
                    //UserId = GetCurrentUserId(),
                    UserId = user.UserId,  // Kullanıcı ID'si
                    CreatedDate = DateTime.Now, // Şu anki tarihi ata
                    //Status = true

                };
                try
                {
                    dbContext.TicketInfoTabs.Add(newTicket);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index", "User");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                }


                //dbContext.TicketInfoTabs.Add(newTicket);

                //await dbContext.SaveChangesAsync();
                //return RedirectToAction("Index", "User");
            }
            return View(model);
        }
        // Destek biletlerinin listelenmesi
        public IActionResult TicketList()
        {
            var userId = int.Parse(User.Identity.Name); // string olan userId'yi int'e dönüştürüyoruz
            var tickets = dbContext.TicketInfoTabs
                .Where(t => t.UserId == userId) // Kullanıcının sadece kendi biletlerini listele
                .ToList();

            return View(tickets);
        }

        // Yorum ekleme
        [HttpGet]
        public IActionResult AddComment(int ticketId)
        {
            ViewBag.TicketId = ticketId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(TicketInfoCommentTab model, int ticketId)
        {
            if (ModelState.IsValid)
            {
                var newComment = new TicketInfoCommentTab
                {
                    TicketId = ticketId,
                    Description = model.Description,
                    CreatedDate = DateTime.Now
                };

                dbContext.TicketInfoCommentTabs.Add(newComment);
                await dbContext.SaveChangesAsync();

                // Yorum eklenince personellere bildirim gönder
                var assignedStaff = dbContext.Users.FirstOrDefault(u => u.UserId == dbContext.TicketInfoTabs.First(t => t.TicketId == ticketId).AssignedPersonId);
                if (assignedStaff != null)
                {
                    var emailSubject = "Yeni Yorum Bileti Üzerine";
                    var emailBody = $"Yeni bir yorum, {ticketId} numaralı biletinize eklenmiştir.";

                    SendEmail(assignedStaff.Email, emailSubject, emailBody);
                }

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Yardımcı metod: E-posta gönderimi
        private void SendEmail(string to, string subject, string body)
        {
            // SMTP üzerinden e-posta gönderme işlemi burada yapılır
        }

        // Yardımcı metod: Giriş yapan kullanıcının ID'sini almak
        private int GetCurrentUserId()
        {
            // Burada, giriş yapan kullanıcının ID'sini almak için ilgili yöntemi kullanın
            // Örneğin, kullanıcıyı Identity üzerinden alabilirsiniz
            return 1; // Örnek olarak 1 döndürülüyor
        }
    }
}
