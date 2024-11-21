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


        //public async Task<IActionResult> Index()
        //{
        //    var tickets = await dbContext.TicketInfoTabs
        //        .Include(t => t.UserTab) 
        //        .ToListAsync();

        //    return View(tickets);
        //}

        public async Task<IActionResult> Index()
        {
            // Giriş yapan kullanıcının adını al
            var currentUserName = User.Identity.Name;

            // Kullanıcı adı üzerinden UserId'yi bul
            var currentUser = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == currentUserName);

            if (currentUser == null)
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }

            // Yalnızca giriş yapan kullanıcının biletlerini getir
            var tickets = await dbContext.TicketInfoTabs
                .Include(t => t.UserTab) // UserTab bilgisi dahil ediliyor
                .Where(t => t.UserId == currentUser.UserId) // Giriş yapan kullanıcının UserId'sine göre filtreleme
                .ToListAsync();

            return View(tickets);
        }

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
                var userName = User.Identity.Name;
                var user = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == userName);
                if (user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                    return View(model);
                }

                var newTicket = new TicketInfoTab
                {
                    Title = model.Title,
                    Description = model.Description,
                    Urgency = model.Urgency,
                    UserId = user.UserId,  
                    CreatedDate = DateTime.Now, 
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


            }
            return View(model);
        }
        // Destek biletlerinin listelenmesi
        public IActionResult TicketList()
        {
            var userId = int.Parse(User.Identity.Name); 
            var tickets = dbContext.TicketInfoTabs
                .Where(t => t.UserId == userId) 
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
            ticket.DeletedDate = DateTime.Now;  
            ticket.Status = false;              

            dbContext.TicketInfoTabs.Update(ticket);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> EditTicket(int id)
        {
            var ticket = await dbContext.TicketInfoTabs
                                        .Include(t => t.UserTab)
                                        .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound("Bilet bulunamadı.");
            }
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);
            if (ticket.AssignedPersonId.HasValue)
            {
                if (ticket.UserTab.UserName != currentUserName)
                {
                    return Forbid("Atanmış bir bilet üzerinde işlem yapma yetkiniz yok.");
                }
                ViewBag.CanEdit = false;  
            }
            else
            {
                ViewBag.CanEdit = true; 
            }

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> EditTicket(TicketInfoTab model)
        {
            var ticket = await dbContext.TicketInfoTabs
       .Include(t => t.UserTab)  
       .FirstOrDefaultAsync(t => t.TicketId == model.TicketId);
            if (ticket == null)
            {
                return NotFound("Bilet bulunamadı.");
            }

            var currentUserName = User.FindFirstValue(ClaimTypes.Name);

            if (ticket.AssignedPersonId.HasValue)
            {
                if (ticket.UserTab != null && ticket.UserTab.UserName != currentUserName)
                {
                    return Forbid("Atanmış bir bilet üzerinde işlem yapma yetkiniz yok.");
                }
            }

            if (ticket.UserTab != null && ticket.UserTab.UserName == currentUserName || ticket.AssignedPersonId == null)

            {
                // Bilet güncelleme
                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.Urgency = model.Urgency;
                ticket.ModifiedDate = DateTime.Now;

                dbContext.TicketInfoTabs.Update(ticket);
                await dbContext.SaveChangesAsync();
                ViewBag.Message = "Bilet başarıyla güncellendi.";
                return RedirectToAction("Index", "User");  


            }
            else
            {
                return Forbid("Bu bilet üzerinde işlem yapma yetkiniz yok.");
            }

        }


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

        //  E-posta gönderimi
        private void SendEmail(string to, string subject, string body)
        {
            // SMTP üzerinden e-posta gönderme işlemi burada yapılcak
        }

        //  Giriş yapan kullanıcının ID'sini almak
        private int GetCurrentUserId()
        {
            // Burada, giriş yapan kullanıcının ID'sini almak için ilgili yöntemi kullanılmalı
            //  kullanıcıyı Identity üzerinden alsak
            return 1; // Örnek olarak 1 döndürülüyor
        }
    }
}
