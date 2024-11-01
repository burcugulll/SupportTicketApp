using System;
using System.ComponentModel.DataAnnotations;
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

        [MaxLength(100)]
        public string Name {  get; set; }

        public byte[] ProfilePhoto { get; set; }

        [Required]
        public UserType UserType { get; set; } 

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Status { get; set; } = true;



    }
}
