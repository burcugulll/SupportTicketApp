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
using Microsoft.AspNetCore.Hosting.Server;
using iText.Commons.Actions.Contexts;
using SupportTicketApp.ViewModels;
using SupportTicketApp.Context;
using SupportTicketApp.Utils;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "Calisan")]
    public class EmployeeController : Controller
    {
        private readonly SupportTicketDbContext _context;
        private readonly EmailService _emailService;



        public EmployeeController(SupportTicketDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;


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

        [HttpGet]
        public async Task<IActionResult> AddComment(int ticketId)
        {
            ViewBag.TicketId = ticketId;

            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.UserTabs.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            var comments = await _context.TicketInfoCommentTabs
                                          .Where(c => c.UserId == user.UserId && c.TicketId == ticketId)
                                          .ToListAsync();

            ViewBag.Comments = comments;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("", "Kullanıcı kimliği bulunamadı.");
                return View(model);
            }
            var user = await _context.UserTabs.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(model);
            }
            var userId = user.UserId;
            
            int ticketId = Convert.ToInt32(Request.Form["TicketId"]);
            var ticket = await _context.TicketInfoTabs
                               .Include(t => t.UserTab) 
                               .FirstOrDefaultAsync(t => t.TicketId == ticketId);
            if (ticket == null)
            {
                ModelState.AddModelError("", "Ticket bulunamadı.");
                return View(model);
            }
            var ticketInfoComment = new TicketInfoCommentTab
            {
                TicketId = ticketId,
                Title = model.Title,
                Description = model.Description,
                CreatedDate = DateTime.Now,
                Status = true,
                UserId = userId
            };

            _context.TicketInfoCommentTabs.Add(ticketInfoComment);
            await _context.SaveChangesAsync();

            if (model.CommentImages != null && model.CommentImages.Count > 0)
            {
                foreach (var image in model.CommentImages)
                {
                    if (image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            var ticketCommentImage = new TicketCommentImage
                            {
                                CommentId = ticketInfoComment.CommentId,
                                ImageData = memoryStream.ToArray(),
                                ContentType = image.ContentType,
                                CreatedDate = DateTime.Now,
                                Status = true
                            };
                            _context.TicketCommentImages.Add(ticketCommentImage);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            // E-posta gönderimi: Biletin sahibi olan kullanıcıya bildirim gönder
            if (ticket.UserTab != null && !string.IsNullOrEmpty(ticket.UserTab.Email))
            {
                try
                {
                    string subject = "Bilet Güncellemesi Bildirimi";
                    string body = $"Merhaba {ticket.UserTab.Name},\n\n" +
                                  $"\"{ticket.Title}\" başlıklı biletiniz güncellenmiştir. Yeni detayları görmek için sisteme giriş yapabilirsiniz.\n\n" +
                                  $"İyi günler dileriz.";

                    await _emailService.SendEmailAsync(ticket.UserTab.Email, subject, body);
                    Console.WriteLine("E-posta başarıyla gönderildi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"E-posta gönderimi sırasında bir hata oluştu: {ex.Message}");
                }
            }
            return RedirectToAction("AssignedTickets", "Employee", new { id = ticketId });
        }

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

        [HttpPost]
        public async Task<IActionResult> BulkUpdateTickets(int[] ticketIds, string action)
        {
            if (ticketIds == null || ticketIds.Length == 0)
            {
                TempData["ErrorMessage"] = "Herhangi bir bilet seçilmedi.";
                return RedirectToAction("AllTickets");
            }

            var ticketIdParams = ticketIds.Select((id, index) => new { Id = id, ParamName = $"@p{index}" }).ToList();

            var parameterizedQuery = $"SELECT * FROM TicketInfoTabs WHERE TicketId IN ({string.Join(", ", ticketIdParams.Select(p => p.ParamName))})";

            var sqlParameters = ticketIdParams
                .Select(p => new Microsoft.Data.SqlClient.SqlParameter(p.ParamName, p.Id))
                .ToArray();

            var tickets = await _context.TicketInfoTabs
                .FromSqlRaw(parameterizedQuery, sqlParameters)
                .ToListAsync();

            if (tickets == null || tickets.Count == 0)
            {
                TempData["ErrorMessage"] = "Seçilen biletler bulunamadı.";
                return RedirectToAction("AllTickets");
            }

            switch (action?.ToLower())
            {
                case "complete":
                    foreach (var ticket in tickets)
                    {

                        ticket.IsCompleted = true;
                        ticket.ModifiedDate = DateTime.Now;
                    }
                    TempData["SuccessMessage"] = $"Seçili {tickets.Count} bilet başarıyla tamamlandı.";
                    break;

                case "delete":
                    foreach (var ticket in tickets)
                    {

                        ticket.Status = false; // Pasif
                        ticket.IsCompleted = true;
                        ticket.DeletedDate = DateTime.Now;
                    }
                    TempData["SuccessMessage"] = $"Seçili {tickets.Count} bilet başarıyla silindi.";
                    break;

                default:
                    TempData["ErrorMessage"] = "Geçersiz bir işlem türü seçildi.";
                    return RedirectToAction("AssignedTickets");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AssignedTickets");

        }
    }
}
