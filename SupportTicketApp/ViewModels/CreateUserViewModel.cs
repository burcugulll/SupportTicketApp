using SupportTicketApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SupportTicketApp.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-posta gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gerekli.")]
        public string Password { get; set; }

        public string Name { get; set; }
        public UserType UserType { get; set; }
    }
}
