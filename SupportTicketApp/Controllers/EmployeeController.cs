using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "Calisan")]
    public class EmployeeController : Controller
    {
        private readonly SupportTicketDbContext _context;


        public EmployeeController(SupportTicketDbContext context)
        {
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AssignedTickets()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Kullanıcı kimliği bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.UserTabs.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var assignedTickets = await _context.TicketAssignments
                .Include(ta => ta.TicketInfoTab)
                .ThenInclude(ti => ti.UserTab)
                .Where(ta => ta.UserId == user.UserId) 
                .Select(ta => ta.TicketInfoTab)
                .ToListAsync();

            return View(assignedTickets);
        }


        //[HttpPost]
        //public async Task<IActionResult> UpdateTickets(int[] selectedTickets, string action)
        //{
        //    if (selectedTickets == null || selectedTickets.Length == 0)
        //    {
        //        // Seçili bilet yoksa hata mesajı dönebiliriz
        //        TempData["ErrorMessage"] = "Lütfen işlem yapmak için bir bilet seçin.";
        //        return RedirectToAction("AssignedTickets"); // Geri döndür
        //    }

        //    if (action == "complete")
        //    {
        //        var tickets = await _context.TicketInfoTabs
        //            .Where(t => selectedTickets.Contains(t.TicketId))
        //            .ToListAsync();

        //        foreach (var ticket in tickets)
        //        {
        //            ticket.IsCompleted = true; // Biletin tamamlandığını işaretle
        //        }

        //        await _context.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Seçili biletler tamamlandı.";
        //    }
        //    else if (action == "delete")
        //    {
        //        var tickets = await _context.TicketInfoTabs
        //            .Where(t => selectedTickets.Contains(t.TicketId))
        //            .ToListAsync();

        //        _context.TicketInfoTabs.RemoveRange(tickets); // Biletleri sil
        //        await _context.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Seçili biletler silindi.";
        //    }

        //    return RedirectToAction("AssignedTickets"); // İşlem tamamlandıktan sonra sayfayı yenile
        //}
        //public async Task<IActionResult> TicketDetails(int id)
        //{
        //    var userId = int.Parse(User.Identity.Name); // Giriş yapan kullanıcının ID'si

        //    // Ticketi ve atanmış kişiyi kontrol et
        //    var ticket = await _context.TicketInfoTabs
        //        .Include(t => t.TicketAssignments)
        //        .FirstOrDefaultAsync(t => t.TicketId == id && t.TicketAssignments.Any(a => a.UserId == userId));

        //    if (ticket == null)
        //    {
        //        TempData["ErrorMessage"] = "Bilet bulunamadı veya bu bilet size atanmadı.";
        //        return RedirectToAction("AssignedTickets");
        //    }

        //    return View(ticket);
        //}


        //[HttpPost]
        //public async Task<IActionResult> AddComment(int TicketId, string Title, string Description)
        //{
        //    var comment = new TicketInfoCommentTab
        //    {
        //        TicketId = TicketId,
        //        Title = Title,
        //        Description = Description,
        //        CreatedDate = DateTime.Now
        //    };

        //    _context.TicketInfoCommentTabs.Add(comment);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(TicketDetails), new { id = TicketId });
        //}

        public IActionResult ExportToPdf(int ticketId)
        {
            var ticket = _context.TicketInfoTabs
                .Include(t => t.UserTab)
                .Include(t => t.TicketImages)
                .Include(t => t.TicketInfoCommentTabs)
                .ThenInclude(c => c.TicketCommentImages)
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
                return NotFound();

            using (var stream = new MemoryStream())
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);
                var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "arial.ttf");

                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                document.Add(new Paragraph($"Bilet ID: {ticket.TicketId}").SetFont(font));
                document.Add(new Paragraph($"Başlık: {ticket.Title}").SetFont(font));
                document.Add(new Paragraph($"Açıklama: {ticket.Description}").SetFont(font));
                document.Add(new Paragraph($"Öncelik: {ticket.Urgency}").SetFont(font));
                document.Add(new Paragraph($"Durum: {(ticket.Status ? "Aktif" : "Pasif")}").SetFont(font));
                document.Add(new Paragraph($"Oluşturma Tarihi: {ticket.CreatedDate.ToString("dd.MM.yyyy")}").SetFont(font));
                document.Add(new Paragraph($"Oluşturan Kullanıcı: {ticket.UserTab?.Name ?? "Bilinmiyor"}").SetFont(font));
                if (ticket.TicketImages != null && ticket.TicketImages.Any())
                {
                    document.Add(new Paragraph("Resimler:").SetFont(font));

                    var imageParagraph = new Paragraph();
                    foreach (var image in ticket.TicketImages)
                    {
                        var imageData = ImageDataFactory.Create(image.ImageData);
                        var pdfImage = new iText.Layout.Element.Image(imageData)
                            .SetWidth(100)
                            .SetHeight(100)
                            .SetMarginRight(10);
                        imageParagraph.Add(pdfImage);
                    }
                    document.Add(imageParagraph);
                }

                if (ticket.TicketInfoCommentTabs != null && ticket.TicketInfoCommentTabs.Any())
                {
                    document.Add(new Paragraph("Yorumlar:").SetFont(font));
                    foreach (var comment in ticket.TicketInfoCommentTabs)
                    {
                        document.Add(new Paragraph($"- {comment.Title}: {comment.Description}").SetFont(font));
                    }
                }

                document.Close();

                return File(stream.ToArray(), "application/pdf", $"Ticket_{ticketId}.pdf");
            }
        }
    }
}
