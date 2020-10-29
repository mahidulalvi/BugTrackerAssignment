using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class ToggleNotificationViewModel
    {
        public List<IndexTicketViewModel> AllUnsubscribedTickets { get; set; }
        public List<IndexTicketViewModel> AllSubsribedTickets { get; set; }
    }
}