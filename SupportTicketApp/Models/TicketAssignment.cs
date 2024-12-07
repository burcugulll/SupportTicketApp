namespace SupportTicketApp.Models
{
    public class TicketAssignment
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool Status { get; set; } // Aktif mi Pasif mi
        public bool IsCompleted { get; set; } // Tamamlandı mı

        // Navigation Properties
        public TicketInfoTab TicketInfoTab { get; set; } // Bir görev bir bilete ait
        public UserTab UserTab { get; set; } // Bir görev bir kullanıcıya ait
    }
}
