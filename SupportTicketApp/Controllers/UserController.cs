using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Models;
using System.Linq;
using System.Security.Claims;
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
        [HttpPost]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var ticket = await dbContext.TicketInfoTabs.FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound("Bilet bulunamadı.");
            }

            if (ticket.AssignedPersonId.HasValue)
            {
                return Forbid("Atanmış bir bilet silinemez.");
            }
            ticket.DeletedDate = DateTime.Now;  // Silinme tarihi ekleniyor
            ticket.Status = false;              // Durum false olarak güncelleniyor

            // Güncellenmiş veriyi kaydediyoruz
            dbContext.TicketInfoTabs.Update(ticket);
            await dbContext.SaveChangesAsync();

            // Güncellenen listeyi göstermek için index sayfasına yönlendiriyoruz
            return RedirectToAction("Index");
            //dbContext.TicketInfoTabs.Remove(ticket);
            //await dbContext.SaveChangesAsync();

            //return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> EditTicket(int id)
        {
            var ticket = await dbContext.TicketInfoTabs
                                        .Include(t => t.UserTab) // Kullanıcı bilgilerini almak için
                                        .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound("Bilet bulunamadı.");
            }
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);
            // Eğer bilet bir personele atanmışsa, sadece yorum eklenebilir
            if (ticket.AssignedPersonId.HasValue)
            {
                // Eğer kullanıcı, atanmış biletin sahibi değilse düzenleme izni verilmeyecek
                if (ticket.UserTab.UserName != currentUserName)
                {
                    return Forbid("Atanmış bir bilet üzerinde işlem yapma yetkiniz yok.");
                }
                // Eğer bilet atanmışsa sadece yorum eklenmesine izin verilmeli
                ViewBag.CanEdit = false;  // Düzenleme yapılmaması için flag ekleyin
            }
            else
            {
                // Bilet atanmadıysa düzenleme yapılabilir
                ViewBag.CanEdit = true; // Düzenleme yapılabilir
            }

            return View(ticket);
        }


        [HttpPost]
        public async Task<IActionResult> EditTicket(TicketInfoTab model)
        {
            //var ticket = await dbContext.TicketInfoTabs.FindAsync(model.TicketId);
            var ticket = await dbContext.TicketInfoTabs
       .Include(t => t.UserTab)  // UserTab'ı da dahil ediyoruz
       .FirstOrDefaultAsync(t => t.TicketId == model.TicketId);
            if (ticket == null)
            {
                return NotFound("Bilet bulunamadı.");
            }

            var currentUserName = User.FindFirstValue(ClaimTypes.Name);

            // Eğer bilet atanmışsa, sadece yorum eklenebilir
            if (ticket.AssignedPersonId.HasValue)
            {
                if (ticket.UserTab != null && ticket.UserTab.UserName != currentUserName)

                //if (ticket.UserTab.UserName != currentUserName)
                {
                    return Forbid("Atanmış bir bilet üzerinde işlem yapma yetkiniz yok.");
                }
            }

            // Kendi biletini düzenleyebilmesi için
            if (ticket.UserTab != null && ticket.UserTab.UserName == currentUserName || ticket.AssignedPersonId == null)

            //if (ticket.UserTab.UserName == currentUserName || ticket.AssignedPersonId == null)
            {
                // Bilet güncelleme
                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.Urgency = model.Urgency;
                ticket.ModifiedDate = DateTime.Now;

                dbContext.TicketInfoTabs.Update(ticket);
                await dbContext.SaveChangesAsync();
                ViewBag.Message = "Bilet başarıyla güncellendi.";
                return RedirectToAction("Index", "User");  // Başarıyla güncellendikten sonra yönlendiriyoruz


            }
            else
            {
                return Forbid("Bu bilet üzerinde işlem yapma yetkiniz yok.");
            }

            //return RedirectToAction("Index", "User");
        }

        //// Yorum ekleme
        //[HttpGet]
        //public IActionResult AddComment(int ticketId)
        //{
        //    ViewBag.TicketId = ticketId;
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string comment)
        {
            var ticket = await dbContext.TicketInfoTabs
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return NotFound("Bilet bulunamadı.");
            }

            // Yorum ekleme işlemi
            var newComment = new TicketInfoCommentTab
            {
                TicketId = ticketId,
                Description = comment,
                CreatedDate = DateTime.Now
            };

            ticket.Comments.Add(newComment);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Details", new { id = ticketId });
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
