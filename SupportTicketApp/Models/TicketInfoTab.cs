﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SupportTicketApp.Enums;
namespace SupportTicketApp.Models
{
    public class TicketInfoTab
    {
        [Key]
        public int TicketId { get; set; }

        [Required, MaxLength(100)]
        public string Title {  get; set; }
        
        public string Description { get; set; }

        public int? TicketImageId { get; set; }

        [MaxLength(20)]
        public UrgencyLevel Urgency { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool Status { get; set; } = true;

        public virtual TicketImage TicketImage { get; set; } // İlişkilendirilen resim
        public virtual ICollection<TicketInfoCommentTab> Comments { get; set; } // Bilete ait yorumlar
    }
}