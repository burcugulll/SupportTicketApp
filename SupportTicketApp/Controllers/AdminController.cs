using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Models;
using System.Linq;
using System.Threading.Tasks;
namespace SupportTicketApp.Controllers
{
    [Authorize(Roles ="Yonetici")]
    public class AdminController : Controller
    {
        private readonly SupportTicketDbContext _context;

        public AdminController(SupportTicketDbContext context)
        {
            _context = context;
        }

        // Bilet listesini görüntüle
        public async Task<IActionResult> TicketManagement(string statusFilter)
        {
            var tickets = from t in _context.TicketInfoTabs
                          select t;

            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool filterStatus;

                if (bool.TryParse(statusFilter, out filterStatus))
                {
                    tickets = tickets.Where(t => t.Status == filterStatus);  
                }
                else
                {
                    tickets = tickets.Where(t => false);
                }
            }

            return View(await tickets.ToListAsync());
        }

        // Bilet detayını görüntüle
        public async Task<IActionResult> TicketDetails(int id)
        {
            var ticket = await _context.TicketInfoTabs
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(m => m.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // Bilet güncelleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicketStatus(int id, string status)
        {
            var ticket = await _context.TicketInfoTabs.FindAsync(id);
            if (ticket != null)
            {
                bool parsedStatus;

                if (bool.TryParse(status, out parsedStatus)) // string'i bool'a dönüştürmeye çalışıyoruz.
                {
                    ticket.Status = parsedStatus;  
                    ticket.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                // E-posta bildirimi (Bonus)
                var userEmail = _context.UserTabs
                    .Where(u => u.UserId == ticket.UserId)
                    .Select(u => u.Email)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(userEmail))
                {
                    // E-posta gönderme işlemi
                    // SendEmail(userEmail, "Bilet Durumu Güncellendi", "Biletiniz güncellenmiştir.");
                }
            }

            return RedirectToAction(nameof(TicketManagement));
        }

        // Biletleri çoklu seçip bitir
        [HttpPost]
        public async Task<IActionResult> MarkAsFinished(int[] selectedTicketIds)
        {
            var tickets = await _context.TicketInfoTabs
                .Where(t => selectedTicketIds.Contains(t.TicketId))
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                ticket.Status = true; // veya başka bir statü
                ticket.ModifiedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TicketManagement));
        }

        // Biletleri sil (statüsü pasife çekilecek)
        [HttpPost]
        public async Task<IActionResult> DeleteTickets(int[] selectedTicketIds)
        {
            var tickets = await _context.TicketInfoTabs
                .Where(t => selectedTicketIds.Contains(t.TicketId))
                .ToListAsync();

            foreach (var ticket in tickets)
            {
                ticket.Status = false; // Pasif statüsü
                ticket.DeletedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TicketManagement));
        }

        // PDF olarak bilet indirme
        [HttpPost]
        public IActionResult DownloadTicketAsPdf(int id)
        {
            var ticket = _context.TicketInfoTabs
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.TicketId == id);

            if (ticket == null)
                return NotFound();

            // PDF oluşturma işlemi (örneğin iTextSharp kullanabilirsiniz)
            // var pdf = CreatePdf(ticket);

            // return File(pdf, "application/pdf", "ticket.pdf");
            return Ok(); // Burada PDF işlemi yapılabilir
        }

        // Personel atama
        [HttpPost]
        public async Task<IActionResult> AssignEmployeeToTicket(int ticketId, int[] employeeIds)
        {
            var ticket = await _context.TicketInfoTabs.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            // Personel ekleme işlemi
            foreach (var empId in employeeIds)
            {
                var user = await _context.UserTabs.FindAsync(empId);
                if (user != null)
                {
                    // Atama işlemi yapılacak
                    // ticket.AssignedEmployees.Add(user);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TicketDetails), new { id = ticketId });
        }
    }
}
    
