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


namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class AdminController : Controller
    {
        private readonly SupportTicketDbContext _context;
        public AdminController(SupportTicketDbContext context)
        {
            _context = context;
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
            return RedirectToAction("Index");

        }
       
        // Tüm Biletler
        public async Task<IActionResult> AllTickets()
        {
            List<TicketInfoTab> ticketInfos = await _context.TicketInfoTabs.ToListAsync();

            foreach (var ticket in ticketInfos)
            {
                ticket.UserTab = await _context.UserTabs.FirstOrDefaultAsync(x => x.UserId == ticket.UserId);
            }


            var tickets = await _context.TicketInfoTabs
                .Include(t => t.UserTab)
                .Include(t => t.TicketImages)
                .Include(t => t.TicketInfoCommentTabs)
                    .ThenInclude(c => c.TicketCommentImages)
                .ToListAsync();

            return View(tickets); 
        }

        // Devam Eden Biletler
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

        // Atanmamış Biletler
        public async Task<IActionResult> UnassignedTickets()
        {
            var tickets = await _context.TicketInfoTabs
        .Where(t => t.UserTab == null && t.IsCompleted == false) 
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

        // Bilet Sil 
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
            // Bileti veritabanından bul
            var ticket = await _context.TicketInfoTabs.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(); // Eğer bilet bulunmazsa, hata döndür
            }

            // Biletin durumunu 'Tamamlandı' olarak işaretle
            ticket.IsCompleted = true;

            //ticket.Status = false; // OngoingTickets'ten silmek için Status değerini false yapıyoruz
            await _context.SaveChangesAsync(); // Değişiklikleri kaydet

            // Biletin bağlı olduğu kullanıcı var mı?
            if (ticket.UserTab == null)
            {
                // Eğer biletin atanmış kullanıcısı yoksa, 'UnassignedTickets' sayfasına yönlendir
                return RedirectToAction(nameof(UnassignedTickets));
            }
            else
            {
                // Bilet tamamlandığı için artık OngoingTickets'te görünmemeli
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

            /*var tickets = await _context.TicketInfoTabs
                .Where(t => ticketIds.Any(id => id == t.TicketId))
                .ToListAsync();*/

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

            //return RedirectToAction("OngoingTickets", "Admin");
        }


        public IActionResult ExportToPdf(int ticketId)
        {
            var ticket = _context.TicketInfoTabs
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
        //[HttpPost]
        //public async Task<IActionResult> AssignUsersToTicket(int ticketId, int userId)
        //{
        //    // Bilet verisini alın
        //    var ticket = await _context.TicketInfoTabs
        //        .Include(t => t.AssignedPerson) // AssignedPerson ile ilişki
        //        .FirstOrDefaultAsync(t => t.TicketId == ticketId);

        //    if (ticket == null)
        //    {
        //        return NotFound(); // Bilet bulunamadı
        //    }

        //    // Kullanıcı verisini alın
        //    var user = await _context.Users
        //        .Include(u => u.AssignedTickets) // AssignedTickets ile ilişki
        //        .FirstOrDefaultAsync(u => u.UserId == userId);

        //    if (user == null)
        //    {
        //        return NotFound(); // Kullanıcı bulunamadı
        //    }

        //    // Bilet ile kullanıcıyı ilişkilendir
        //    if (!ticket.AssignedPerson.Contains(user))
        //    {
        //        ticket.AssignedPerson.Add(user); // Bilete kullanıcıyı ekle
        //    }

        //    if (!user.AssignedTickets.Contains(ticket))
        //    {
        //        user.AssignedTickets.Add(ticket); // Kullanıcıya bileti ekle
        //    }

        //    // Değişiklikleri kaydet
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "Personel başarıyla atandı!";
        //    return RedirectToAction("TicketDetails", new { ticketId });
        //}



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
                        TicketId = ticket.TicketId,  // İlgili biletin TicketId'sini ayarlayın
                        UserId = user.UserId,        // Kullanıcının UserId'sini ayarlayın
                                                     // Diğer gerekli alanları da buraya ekleyebilirsiniz (varsa)
                    };

                    ticket.TicketAssignments.Add(ticketAssignment);  // TicketAssignment nesnesini koleksiyona ekleyin
                }
            }


            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Personel başarıyla atandı!";

            return RedirectToAction("TicketDetails", new { ticketId });
        }



        //[HttpPost]
        //public async Task<IActionResult> AssignUsersToTicket(int ticketId, int[] userIds)
        //{
        //    var ticket = await _context.TicketInfoTabs
        //        .Include(t => t.AssignedPerson)
        //        .FirstOrDefaultAsync(t => t.TicketId == ticketId);

        //    if (ticket == null)
        //        return NotFound();

        //    foreach (var userId in userIds)
        //    {
        //        if (!ticket.AssignedPerson.Any(u => u.UserId == userId))
        //        {
        //            ticket.AssignedPerson.Add(new UserTab { UserId = userId });
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("TicketDetail", new { id = ticketId });
        //}
    }
}
    
 

    
