using System;
using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.Models
{
    public class UserLogTab
    {
        [Key]
        public int LogId {  get; set; }

        [Required, MaxLength(50)]
        public string UserName {  get; set; }
        public DateTime LogTime { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string IPAdress { get; set; }

        public string Log {  get; set; }
    }
}
