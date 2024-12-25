using System;
using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.Models
{
    public class TicketCommentImage
    {
        [Key]
        public int CommentImageId { get; set; }
        public byte[] ImageData { get; set; } 
        public string ContentType { get; set; } // İçeriğin MIME tipi (örneğin, "image/jpeg")
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
        public bool Status { get; set; } = true;
        public int CommentId { get; set; }  


        // Navigation properties
        public TicketInfoCommentTab TicketInfoCommentTab { get; set; }
    }
}
