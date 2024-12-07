using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Models;
using System;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "Calisan")]

    public class EmployeeController :Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly SupportTicketDbContext _context;

        public EmployeeController(SupportTicketDbContext context)
        {
            _context = context;
        }
        public IActionResult AssignedTickets()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                TempData["ErrorMessage"] = "Geçerli bir kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var assignedTickets = _context.TicketInfoTabs
                .Where(t => t.UserId == userId) // UserTabUserId ile eşleşen biletler
                .ToList();

            return View(assignedTickets);
        }

        //public IActionResult AssignedTickets(string ticketId)
        //{
        //    // Kullanıcı ID'sini güvenli şekilde almak
        //    var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        //    if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        //    {
        //        TempData["ErrorMessage"] = "Geçerli bir kullanıcı kimliği bulunamadı.";
        //        return RedirectToAction("Index", "Home"); // Hatalı durum için yönlendirme
        //    }




        //    int parsedTicketId;

        //    // ticketId'nin int'e dönüşümünü yaparken hata kontrolü
        //    if (!int.TryParse(ticketId, out parsedTicketId))
        //    {
        //        TempData["ErrorMessage"] = "Geçersiz bilet ID'si.";
        //        return RedirectToAction("AssignedTickets");
        //    }

        //    // TicketId'yi doğru bir şekilde kullanarak işlemlere devam et
        //    var ticket = _context.TicketInfoTabs.FirstOrDefault(t => t.TicketId == parsedTicketId && t.AssignedPerson.Any(u => u.UserId == userId));

        //    if (ticket == null)
        //    {
        //        TempData["ErrorMessage"] = "Bilet bulunamadı veya bu bilet size atanmış değil.";
        //        return RedirectToAction("AssignedTickets");
        //    }

        //    return View(ticket);
        //}




        //public async Task<IActionResult> AssignedTickets()
        //{
        //    var userId = int.Parse(User.Identity.Name); // Giriş yapan kullanıcının ID'sini al

        //    // Atanmış biletleri al
        //    var tickets = await _context.TicketInfoTabs
        //        .Where(t => t.AssignedPerson.Any(a => a.UserId == userId) && t.Status)
        //        .ToListAsync();

        //    return View(tickets); // Atanmış biletleri görüntüle
        //                          //// Giriş yapan kullanıcının ID'sini al
        //                          //var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        //    //if (int.TryParse(userId, out int parsedUserId))
        //    //{
        //    //    // Giriş yapan kullanıcının atanmış olduğu biletleri al
        //    //    var tickets = await _context.TicketInfoTabs
        //    //        .Where(t => t.AssignedPerson.Any(u => u.UserId == parsedUserId) && t.Status) // Çalışanın atanmış aktif biletleri
        //    //        .ToListAsync();

        //    //    return View(tickets);
        //    //}
        //    //else
        //    //{
        //    //    // Eğer userId geçerli değilse, uygun bir hata mesajı gösterebilirsiniz.
        //    //    return BadRequest("Geçersiz kullanıcı ID.");
        //    //}




        //    //var userId = int.Parse(User.Identity.Name);
        //    ////var userId = User.Identity.GetUserId<int>(); // Giriş yapan kullanıcının ID'sini al
        //    //var tickets = await _context.TicketInfoTabs
        //    //    .Where(t => t.AssignedPerson.Any(u => u.UserId == userId) && t.Status) // Çalışanın atanmış aktif biletleri
        //    //    .ToListAsync();

        //    //return View(tickets);
        //}

        [HttpPost]
        public async Task<IActionResult> UpdateTickets(int[] selectedTickets, string action)
        {
            if (selectedTickets == null || selectedTickets.Length == 0)
            {
                // Seçili bilet yoksa hata mesajı dönebiliriz
                TempData["ErrorMessage"] = "Lütfen işlem yapmak için bir bilet seçin.";
                return RedirectToAction("AssignedTickets"); // Geri döndür
            }

            if (action == "complete")
            {
                var tickets = await _context.TicketInfoTabs
                    .Where(t => selectedTickets.Contains(t.TicketId))
                    .ToListAsync();

                foreach (var ticket in tickets)
                {
                    ticket.IsCompleted = true; // Biletin tamamlandığını işaretle
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Seçili biletler tamamlandı.";
            }
            else if (action == "delete")
            {
                var tickets = await _context.TicketInfoTabs
                    .Where(t => selectedTickets.Contains(t.TicketId))
                    .ToListAsync();

                _context.TicketInfoTabs.RemoveRange(tickets); // Biletleri sil
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Seçili biletler silindi.";
            }

            return RedirectToAction("AssignedTickets"); // İşlem tamamlandıktan sonra sayfayı yenile
        }
        public async Task<IActionResult> TicketDetails(int id)
        {
            var userId = int.Parse(User.Identity.Name); // Giriş yapan kullanıcının ID'si

            // Ticketi ve atanmış kişiyi kontrol et
            var ticket = await _context.TicketInfoTabs
                .Include(t => t.TicketAssignments)
                .FirstOrDefaultAsync(t => t.TicketId == id && t.TicketAssignments.Any(a => a.UserId == userId));

            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Bilet bulunamadı veya bu bilet size atanmadı.";
                return RedirectToAction("AssignedTickets");
            }

            return View(ticket);
        }

        //public async Task<IActionResult> TicketDetails(int id)
        //{
        //    var ticket = _context.TicketInfoTabs
        //        .Include(t => t.AssignedPerson)
        //        .Include(t => t.Comments)      // Comments ilişkisini dahil et
        //        .FirstOrDefault(t => t.TicketId == id);

        //    if (ticket == null)
        //        return NotFound();

        //    return View(ticket);
        //}

        [HttpPost]
        public async Task<IActionResult> AddComment(int TicketId, string Title, string Description)
        {
            var comment = new TicketInfoCommentTab
            {
                TicketId = TicketId,
                Title = Title,
                Description = Description,
                CreatedDate = DateTime.Now
            };

            _context.TicketInfoCommentTabs.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TicketDetails), new { id = TicketId });
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            // PDF oluşturma ve indirme işlemi burada yapılacak
            return File(new byte[0], "application/pdf", "ticket-details.pdf");
        }
    }
}
