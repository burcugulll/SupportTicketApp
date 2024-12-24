using System.ComponentModel.DataAnnotations;

namespace SupportTicketApp.ViewModels
{
    public class CreateCommentViewModel
    {

        public int TicketId { get; set; }

        [MaxLength(100)]
        public string CommentTitle { get; set; }

        [Required]
        public string CommentDescription { get; set; }

        public List<IFormFile> CommentImages { get; set; } = new List<IFormFile>();

    }
}
