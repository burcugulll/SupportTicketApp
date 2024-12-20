using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SupportTicketApp.Context;
using SupportTicketApp.Models;
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

        public UserController(SupportTicketDbContext context)
        {
            dbContext = context;
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

        // Edit Ticket
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
        public async Task<IActionResult> AddComment(int ticketId, string comment)
        {
            var ticket = await dbContext.TicketInfoTabs
                .Include(t => t.TicketInfoCommentTabs)
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

            ticket.TicketInfoCommentTabs.Add(newComment);
            // Send email notification to assigned personnel if applicable
            foreach (var assignment in ticket.TicketAssignments)
            {
                var userEmail = dbContext.UserTabs.SingleOrDefault(u => u.UserId == assignment.UserId)?.Email;
                if (userEmail != null)
                {
                    SendEmail(userEmail, "Yeni Bir Yorum Eklendi", "Biletinize yeni bir yorum eklendi.");
                }
            }

            return RedirectToAction("Details", new { id = ticketId });
        }

        private void SendEmail(string to, string subject, string body)
        {
            try
            {
                // SMTP sunucusu ayarları
                var smtpClient = new SmtpClient("smtp.yourmailserver.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                    EnableSsl = true,
                };

                // E-posta mesajı
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@example.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                // SMTP üzerinden e-posta gönderimi
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama veya hata mesajı göstermek için buraya kod yazabilirsiniz
                Console.WriteLine($"E-posta gönderme hatası: {ex.Message}");
            }
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
