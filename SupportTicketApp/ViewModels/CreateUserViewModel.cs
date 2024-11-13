using SupportTicketApp.Enums;

namespace SupportTicketApp.ViewModels
{
    public class CreateUserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public byte[] ProfilePhoto { get; set; }

        public UserType UserType { get; set; }
    }
}
