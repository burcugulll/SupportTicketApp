using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace SupportTicketApp.Models
{
    public class TicketInfoCommentTab
    {
        [Key]
        public int CommentId { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool Status { get; set; } = true;

        [ForeignKey(nameof(UserTab))]
        public int UserId { get; set; }
        public UserTab UserTab { get; set; }

        public int TicketId { get; set; } 
        public TicketInfoTab TicketInfoTab { get; set; } // Bire çok: Her yorum bir bilete ait

        public ICollection<TicketCommentImage?> TicketCommentImages { get; set; }

        
    }
}
