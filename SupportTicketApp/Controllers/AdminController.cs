using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Enums;
using SupportTicketApp.Models;
using SupportTicketApp.ViewModels;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Commons.Actions.Contexts;
using Microsoft.Extensions.Primitives;
using iText.IO.Image;
using SupportTicketApp.Context;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using SupportTicketApp.Utils;


namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class AdminController : Controller
    {
        private readonly SupportTicketDbContext _context;
        private readonly EmailService _emailService;

        public AdminController(SupportTicketDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        public IActionResult Index()
        {
            ViewData["Title"] = "Admin Paneli";
            return View();
        }

        public IActionResult UserLog()
        {
            var userLogs = _context.UserLogTabs.ToList();
            return View(userLogs);
        }

        public IActionResult CreateUser()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
           

            var user = new UserTab 
            {
                UserName = model.UserName,
                Email = model.Email,
                
                UserType = model.UserType,
                Name = model.Name, 
                CreatedDate = DateTime.Now,
                Status = true,
                
                ProfilePhoto = new byte[0] 


            };
            string salt = SupportTicketApp.Models.UserTab.GenerateSalt();
            user.Salt = salt;
            user.Password = user.HashPassword(model.Password);
            
            _context.UserTabs.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
            return RedirectToAction("UserManagement", "Admin");

        }

        public async Task<IActionResult> AllTickets()
        {
            var tickets = await _context.TicketInfoTabs
                .Include(t => t.UserTab) 
                .Include(t => t.TicketImages) 
                .Include(t => t.TicketInfoCommentTabs) 
                    .ThenInclude(c => c.TicketCommentImages) 
                .ToListAsync();

            return View(tickets);
        }

        public async Task<IActionResult> OngoingTickets()
        {
            var tickets = await _context.TicketInfoTabs
        .Where(t => t.IsCompleted == false) 
                .Include(t => t.UserTab)
                .Include(t => t.TicketImages)
                .Include(t => t.TicketInfoCommentTabs)
                    .ThenInclude(c => c.TicketCommentImages)
                .ToListAsync();

            return View(tickets); 
        }

        public async Task<IActionResult> UnassignedTickets()
        {
            var tickets = await _context.TicketInfoTabs
        .Where(t => !t.TicketAssignments.Any() && !t.IsCompleted) // Biletler, hiç atanmamış ve tamamlanmamış olmalı
                .Include(t => t.UserTab)
                .Include(t => t.TicketImages)
                .Include(t => t.TicketInfoCommentTabs)
                    .ThenInclude(c => c.TicketCommentImages)
                .ToListAsync();

            return View(tickets); 
        }
        public async Task<IActionResult> UserManagement()
        {
            var users = await _context.UserTabs
        .Where(u => u.UserType != UserType.Yonetici)  
        .ToListAsync();
            return View(users);
        }


        public IActionResult TicketDetails(int ticketId)
        {
            var ticket = _context.TicketInfoTabs
                .Include(t => t.UserTab)
                .Include(t => t.TicketImages)
                .Include(t => t.TicketAssignments)
                .Include(t => t.TicketInfoCommentTabs)
                .FirstOrDefault(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }
            ViewBag.Users = _context.UserTabs
                .Where(u => u.UserType == UserType.Calisan)
       .Select(u => new { u.UserId, u.Name })
       .ToList();

            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> EditTicket(int ticketId)
        {
            var ticket = await _context.TicketInfoTabs
                .Include(t => t.TicketImages)
                .SingleOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = new CreateTicketViewModel
            {
                Title = ticket.Title,
                Description = ticket.Description,
                Urgency = ticket.Urgency,
                TicketImages = null
            };
            ViewBag.TicketId = ticket.TicketId; 

            ViewBag.TicketImages = ticket.TicketImages;

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditTicket(int ticketId, CreateTicketViewModel model)
        {

            var ticket = await _context.TicketInfoTabs
                .Include(t => t.TicketImages)
                .Include(t => t.UserTab)
                .SingleOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.Urgency = model.Urgency;

            if (model.TicketImages != null && model.TicketImages.Any())
            {
                foreach (var image in model.TicketImages)
                {
                    var newTicketImage = new TicketImage();

                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        newTicketImage.ImageData = memoryStream.ToArray();
                        newTicketImage.ContentType = image.ContentType;
                        newTicketImage.Status = true;
                        newTicketImage.TicketId = ticket.TicketId;
                    }

                    _context.TicketImages.Add(newTicketImage);
                }
            }

            // Update existing images (soft delete)
            if (!StringValues.IsNullOrEmpty(Request.Form["DeletedImageIds"]))
            {
                var deletedImageIds = Request.Form["DeletedImageIds"]
                    .ToString()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                var imagesToDelete = ticket.TicketImages
                    .Where(img => deletedImageIds.Contains(img.TicketImageId))
                    .ToList();

                foreach (var img in imagesToDelete)
                {
                    img.Status = false;  
                    img.DeletedDate = DateTime.Now;  
                }

                _context.TicketImages.UpdateRange(imagesToDelete);  
            }

            await _context.SaveChangesAsync();
            // E-posta gönderimi
            if (ticket.UserTab != null && !string.IsNullOrEmpty(ticket.UserTab.Email))
            {
                string subject = "Bilet Güncellemesi Bildirimi";
                string body = $"Merhaba {ticket.UserTab.Name},\n\n" +
                              $"\"{ticket.Title}\" başlıklı biletiniz güncellenmiştir. Yeni detayları görmek için sisteme giriş yapabilirsiniz.\n\n" +
                              $"İyi günler dileriz.";

                await _emailService.SendEmailAsync(ticket.UserTab.Email, subject, body);
            }

            return RedirectToAction("TicketDetails", new { ticketId = ticket.TicketId });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string commentTitle, string commentDescription)
        {
            var ticket = await _context.TicketInfoTabs.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            var newComment = new TicketInfoCommentTab
            {
                Title = commentTitle,
                Description = commentDescription,
                TicketId = ticketId,
                CreatedDate = DateTime.Now,
                Status = true
            };

            _context.TicketInfoCommentTabs.Add(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("TicketDetail", new { id = ticketId });
        }


        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.UserTabs.FindAsync(id);
            if (user == null)
                return NotFound();

            //_context.UserTabs.Remove(user);
            user.Status = false;
            user.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }

        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var ticket = await _context.TicketInfoTabs.FindAsync(ticketId);
            if (ticket == null)
                return NotFound();

            ticket.Status = false;
            ticket.IsCompleted = true;
            ticket.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            if (ticket.UserTab == null) 
            {
                return RedirectToAction(nameof(UnassignedTickets));
            }
            else if (ticket.Status == true) 
            {
                return RedirectToAction(nameof(OngoingTickets));
            }
            else 
            {
                return RedirectToAction(nameof(AllTickets));
            }
        }

        public async Task<IActionResult> CompleteTicket(int ticketId)
        {
            var ticket = await _context.TicketInfoTabs.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(); 
            }

            ticket.IsCompleted = true;

            await _context.SaveChangesAsync(); 

            if (ticket.UserTab == null)
            {
                return RedirectToAction(nameof(UnassignedTickets));
            }
            else
            {
                return RedirectToAction(nameof(OngoingTickets));
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
                    return RedirectToAction("AllTickets");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("AllTickets"); 

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
        public async Task<IActionResult> AssignUsersToTicket(int ticketId, int[] userIds)
        {
            var ticket = await _context.TicketInfoTabs
                .Include(t => t.TicketAssignments)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            if(userIds.Length == 0)
            {
                TempData["ErrorMessage"] = "Personel Atanamadı, Tekrar Deneyin!";
                return RedirectToAction("TicketDetails", new { ticketId });
            }

            foreach (var userId in userIds)
            {
                var user = await _context.UserTabs.FindAsync(userId);
                if (user != null && !ticket.TicketAssignments.Any(a => a.UserId == userId))
                {
                    var ticketAssignment = new TicketAssignment
                    {
                        TicketId = ticket.TicketId,  
                        UserId = user.UserId,
                        AssignedDate = DateTime.Now ,
                        Status=ticket.Status,
                        IsCompleted=ticket.IsCompleted
                                                     
                    };

                    ticket.TicketAssignments.Add(ticketAssignment);  
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Personel başarıyla atandı!";

            return RedirectToAction("TicketDetails", new { ticketId });
        }

        [HttpPost]
        public IActionResult ActivateUser(int id)
        {
            // Kullanıcıyı veritabanında bul ve aktif et
            var user = _context.UserTabs.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                user.Status = true;  // Kullanıcıyı aktif hale getir
                _context.SaveChanges();  // Değişiklikleri kaydet
                TempData["SuccessMessage"] = "Kullanıcı başarıyla aktifleştirildi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı!";
            }

            return RedirectToAction("UserManagement"); // Kullanıcı yönetimi sayfasına yönlendir
        }


    }
}
    
 

    
