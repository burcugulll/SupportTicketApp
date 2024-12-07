using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;
using SupportTicketApp.Enums;
namespace SupportTicketApp.Models

{
    public class UserTab
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string UserName { get; set; }

        [Required, MaxLength(50)]
        public string Password { get; set; }
        public string Salt { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }


        [MaxLength(100)]
        public string Name {  get; set; }

        public byte[] ProfilePhoto { get; set; }

        [Required]
        public UserType UserType { get; set; } 

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Status { get; set; } = true;
        public int LoginAttempts { get; set; }
        public DateTime? LockoutEndTime { get; set; }
        // Navigation Properties
        public ICollection<TicketInfoTab> TicketInfoTabs { get; set; } // Bire çok: Kullanıcının oluşturduğu biletler
        public ICollection<UserLogTab> UserLogTabs { get; set; } // Bire çok: Kullanıcının logları
        public ICollection<TicketAssignment> TicketAssignments { get; set; } // Çoka çok: Kullanıcının atanmış biletleri




        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));
                return Convert.ToBase64String(bytes);
            }
        }

        public static string GenerateSalt()
        {
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
        }

    }
}
