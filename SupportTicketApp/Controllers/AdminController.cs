using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Enums;
using SupportTicketApp.Models;
using System.Linq;
using System.Threading.Tasks;
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

        
        // Tüm Biletler
        public async Task<IActionResult> AllTickets()
        {
            var tickets = await _context.TicketInfoTabs
                .Include(t => t.UserTab)
                .Include(t => t.TicketImage)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CommentImages)
                .ToListAsync();

            return View(tickets); 
        }

        // Devam Eden Biletler
        public async Task<IActionResult> OngoingTickets()
        {
            var tickets = await _context.TicketInfoTabs
                .Where(t => t.Status == true) 
                .Include(t => t.UserTab)
                .Include(t => t.TicketImage)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CommentImages)
                .ToListAsync();

            return View(tickets); 
        }

        // Atanmamış Biletler
        public async Task<IActionResult> UnassignedTickets()
        {
            var tickets = await _context.TicketInfoTabs
                .Where(t => t.UserTab == null) 
                .Include(t => t.UserTab)
                .Include(t => t.TicketImage)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CommentImages)
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
        


        public async Task<IActionResult> TicketDetail(int id)
        {
            var ticket = await _context.TicketInfoTabs
                .Include(t => t.UserTab) 
                .Include(t => t.TicketImage) 
                .Include(t => t.Comments) 
                    .ThenInclude(c => c.CommentImages) 
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

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


        // Kullanıcı Sil
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
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.TicketInfoTabs.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.Status = false;
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
    }
}
    
 

    
