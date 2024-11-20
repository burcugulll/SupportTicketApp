using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SupportTicketApp.Controllers
{
    [Authorize(Roles = "Calisan")]

    public class EmployeeController :Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
