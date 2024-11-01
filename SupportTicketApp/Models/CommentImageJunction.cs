namespace SupportTicketApp.Models
{
    public class CommentImageJunction
    {
        public int Id { get; set; }
        public int CommentId { get; set; } // Hangi yoruma ait
        public int ImageId { get; set; } // Hangi resme ait

        // Foreign key ilişkileri
        public virtual TicketInfoCommentTab TicketComment { get; set; } // İlişkilendirilen yorum
        public virtual TicketCommentImage TicketCommentImage { get; set; } // İlişkilendirilen resim
    }
}
