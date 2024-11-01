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
        public int? TicketCommentImageId { get; set; }
        public virtual TicketCommentImage TicketCommentImage { get; set; } // İlişkilendirilen yorum resmi

        public DateTime? DeletedDate { get; set; }

        public bool Status { get; set; } = true;

        public int TicketId { get; set; } // Hangi biletle ilişkili
        public virtual TicketInfoTab Ticket { get; set; } // İlişkilendirilen bilet
        public virtual ICollection<CommentImageJunction> CommentImages { get; set; } // Yorum için birden fazla resim
    }
}
