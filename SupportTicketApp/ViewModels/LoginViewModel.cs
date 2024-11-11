using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Lütfen kullanıcı adı veya e-posta giriniz.")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Lütfen şifre giriniz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
