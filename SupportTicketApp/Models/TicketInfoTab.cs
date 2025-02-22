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

        [EnumDataType(typeof(UrgencyLevel))]
        public UrgencyLevel Urgency { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public bool Status { get; set; } = true;
        public bool IsCompleted { get; set; }
        public int UserId { get; set; } // FK to User
        public UserTab UserTab { get; set; } // Bire çok: Her bilet bir kullanıcıya ait

        public ICollection<TicketImage?> TicketImages { get; set; }
        public ICollection<TicketInfoCommentTab?> TicketInfoCommentTabs { get; set; }
        public ICollection<TicketAssignment?> TicketAssignments { get; set; } // Çoka çok: Birden çok personele atanabilir
                                                                             


        
    }
}
