using SupportTicketApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SupportTicketApp.ViewModels
{
    public class CreateTicketViewModel
    {
        public string Title { get; set; }

        [Required(ErrorMessage = "Açıklama gerekli.")]
        public string Description { get; set; }

        [EnumDataType(typeof(UrgencyLevel))]
        public UrgencyLevel Urgency { get; set; }

        public ICollection<IFormFile> TicketImages { get; set; }

    }
}
