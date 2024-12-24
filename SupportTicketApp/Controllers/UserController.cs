using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SupportTicketApp.Context;
using SupportTicketApp.Models;
using SupportTicketApp.Utils;
using SupportTicketApp.ViewModels;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "SonKullanici")]

    public class UserController : Controller
    {
        private readonly SupportTicketDbContext dbContext;
        private readonly EmailService _emailService;


        public UserController(SupportTicketDbContext context, EmailService emailService)
        {
            dbContext = context;
            _emailService = emailService;

        }

        public async Task<IActionResult> Index()
        {
            var currentUserName = User.Identity.Name;
            var currentUser = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == currentUserName);

            if (currentUser == null)
            {
                return Unauthorized("Kullanıcı bulunamadı.");
            }

            var tickets = await dbContext.TicketInfoTabs
                .Include(t => t.UserTab) 
                .Where(t => t.UserId == currentUser.UserId) 
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
        public async Task<IActionResult> CreateTicket(CreateTicketViewModel model)
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
                CreatedDate = DateTime.Now
            };

            try
            {
                dbContext.TicketInfoTabs.Add(newTicket);
                await dbContext.SaveChangesAsync();

                var newTicketImages = new List<TicketImage>();

                // Dosya ekleniyorsa
                if (model.TicketImages != null && model.TicketImages.Any())
                {
                    foreach (var image in model.TicketImages)
                    {
                        var newTicketImage = new TicketImage();

                        try
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await image.CopyToAsync(memoryStream);
                                newTicketImage.ImageData = memoryStream.ToArray();
                                newTicketImage.ContentType = image.ContentType;
                                newTicketImage.Status = true;
                                newTicketImage.TicketId = newTicket.TicketId;
                            }

                            newTicketImages.Add(newTicketImage);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Resim yükleme sırasında hata oluştu: " + ex.Message);
                        }
                    }
                }
                dbContext.TicketImages.AddRange(newTicketImages);
                await dbContext.SaveChangesAsync();

                //SendEmail(user.Email, "Yeni Bilet Oluşturdunuz", "Yeni bir destek bileti oluşturduğunuz için teşekkür ederiz.");

                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bilet oluşturma sırasında bir hata oluştu: " + ex.Message);
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

            if (ticket.TicketAssignments != null && ticket.TicketAssignments.Any())
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
                .Include(t => t.TicketImages)
                .SingleOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }
            var assignedUser = await dbContext.TicketAssignments
        .FirstOrDefaultAsync(ta => ta.TicketId == id);
            bool canEdit = assignedUser == null;
            // Eğer bilet atanmış bir personele sahipse, düzenlemeyi engelle
            ViewBag.CanEdit = canEdit; // Düzenleme yapılabilir mi, bunu View'e gönderiyoruz
            ViewBag.TicketId = ticket.TicketId;

            var model = new CreateTicketViewModel
            {
                Title = ticket.Title,
                Description = ticket.Description,
                Urgency = ticket.Urgency,
                TicketImages = null
            };

            ViewBag.TicketImages = ticket.TicketImages;

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditTicket(int id, CreateTicketViewModel model)
        {
            var ticket = await dbContext.TicketInfoTabs
                .Include(t => t.TicketImages)
                .SingleOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }
            var currentUserName = User.Identity.Name;
            var currentUser = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == currentUserName);
            var assignedUser = await dbContext.TicketAssignments
            .FirstOrDefaultAsync(ta => ta.TicketId == id);
            // Eğer bilet atanmış bir kullanıcıya sahipse, düzenlemeyi engelle
            if (assignedUser != null)
            {
                ViewData["ErrorMessage"] = "Bu bilet bir personele atanmış, düzenlenemez.";
                return View(); // Hata mesajını View'de göstereceğiz
                //return View("Error", new { message = "Bu bilet bir personele atanmış, düzenlenemez." });
            }

            ticket.Title = model.Title;
            ticket.Description = model.Description;
            ticket.Urgency = model.Urgency;

            // Yeni resimler ekle
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

                    dbContext.TicketImages.Add(newTicketImage);
                }
            }

            // Mevcut resimlerden silme işlemi(soft delete)
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

                dbContext.TicketImages.UpdateRange(imagesToDelete);  // Güncelleme işlemi, sadece durumu pasif yap
            }

            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "User");
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentViewModel model)
        {
            var ticket = await dbContext.TicketInfoTabs
                  .Include(t => t.UserTab) // UserTab'ı yükle
                  .Include(t => t.TicketAssignments) // Atanmış personel bilgilerini yükle
                .SingleOrDefaultAsync(t => t.TicketId == model.TicketId);

            if (ticket == null)
            {
                return NotFound();
            }

            var currentUserName = User.Identity.Name;
            var currentUser = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserName == currentUserName);

            var comment = new TicketInfoCommentTab
            {
                TicketId = ticket.TicketId,
                Title = model.CommentTitle,
                Description = model.CommentDescription,
                CreatedDate = DateTime.Now,
                UserId = currentUser.UserId
            };

            dbContext.TicketInfoCommentTabs.Add(comment);
            await dbContext.SaveChangesAsync();

            // Save comment images
            if (model.CommentImages != null && model.CommentImages.Count > 0)
            {
                foreach (var file in model.CommentImages)
                {
                    var commentImage = new TicketCommentImage();
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        commentImage.ImageData = memoryStream.ToArray();
                        commentImage.ContentType = file.ContentType;
                        commentImage.CommentId = comment.CommentId;
                    }

                    dbContext.TicketCommentImages.Add(commentImage);
                }
            }

            await dbContext.SaveChangesAsync();
            // Atanmış personellere e-posta gönderimi
            foreach (var assignment in ticket.TicketAssignments)
            {
                var assignedUser = await dbContext.UserTabs.SingleOrDefaultAsync(u => u.UserId == assignment.UserId);
                if (assignedUser != null && !string.IsNullOrEmpty(assignedUser.Email))
                {
                    try
                    {
                        string subject = "Yeni Yorum Bildirimi";
                        string body = $"Merhaba {assignedUser.Name},\n\n" +
                                      $"\"{ticket.Title}\" başlıklı biletinize yeni bir yorum eklenmiştir. Yeni detayları görmek için sisteme giriş yapabilirsiniz.\n\n" +
                                      $"İyi günler dileriz.";

                        await _emailService.SendEmailAsync(assignedUser.Email, subject, body);
                        Console.WriteLine($"Atanmış personele e-posta başarıyla gönderildi: {assignedUser.Email}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"E-posta gönderimi sırasında bir hata oluştu: {ex.Message}");
                    }
                }
            }
            return RedirectToAction("EditTicket", new { id = model.TicketId });
        }


    }
}
