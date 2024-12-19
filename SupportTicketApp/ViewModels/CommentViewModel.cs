using SupportTicketApp.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupportTicketApp.ViewModels
{
    public class CommentViewModel
    {

        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public List<IFormFile> CommentImages { get; set; } = new List<IFormFile>();
    }
}

