using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportTicketApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "SonKullanici")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View(); 
        }
    }
}
