using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı en az 3 ve en fazla 50 karakter olmalıdır.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Şifre en az 5 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifreyi onaylayın.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Adı soyadı gereklidir.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Adınız en az 3 ve en fazla 100 karakter olmalıdır.")]
        public string Name { get; set; }
    }
}
