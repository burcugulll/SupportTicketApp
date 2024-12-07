using System;
using System.ComponentModel.DataAnnotations;
namespace SupportTicketApp.Models
{
    public class TicketCommentImage
    {
        [Key]
        public int CommentImageId { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }
        public bool Status { get; set; } = true;
        public int CommentId { get; set; }  // Bu satır eklendi


        // Navigation properties
        public TicketInfoCommentTab TicketInfoCommentTab { get; set; }
    }
}
