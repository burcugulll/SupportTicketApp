using System;
using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.Models
{
    public class TicketImage
    {

        [Key]
        public int TicketImageId { get; set; }

        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
        public bool Status { get; set; } = true;

        public int TicketId { get; set; }  // Bu satır eklendi


        // Navigation properties
        public TicketInfoTab TicketInfoTab { get; set; }

    }
}
