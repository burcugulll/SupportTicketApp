using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.ViewModels
{
    public class LoginViewModel
    {
        [MinLength(3)]
        public string UserName { get; set; }

        [MinLength(5)]
        public string Password { get; set; }
    }
}
