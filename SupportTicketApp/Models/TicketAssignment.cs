namespace SupportTicketApp.Models
{
    public class TicketAssignment
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool Status { get; set; } 
        public bool IsCompleted { get; set; } 

        // Navigation Properties
        public TicketInfoTab TicketInfoTab { get; set; }
        public UserTab UserTab { get; set; } 
    }
}
