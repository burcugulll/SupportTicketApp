using System;
using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.Models
{
    public class TicketImage
    {
        [Key]
        public int ImageId { get; set; }

        public string ImageUrl { get; set; } // Resmin URL'si

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool Status { get; set; } = true;

        // İlişkilendirilen TicketInfoTab
        public virtual TicketInfoTab Ticket { get; set; }
    }
}
