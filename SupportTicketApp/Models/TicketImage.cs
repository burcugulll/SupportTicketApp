using System;
using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.Models
{
    public class TicketImage
    {

        [Key]
        public int TicketImageId { get; set; }

        public byte[] ImageData { get; set; } 
        public string ContentType { get; set; } // İçeriğin MIME tipi (örneğin, "image/jpeg")
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
        public bool Status { get; set; } = true;

        public int TicketId { get; set; } 


        // Navigation properties
        public TicketInfoTab TicketInfoTab { get; set; }

    }
}
