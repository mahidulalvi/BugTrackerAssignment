using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ToggleNotificationViewModel
    {
        public List<IndexTicketViewModel> AllUnsubscribedTickets { get; set; }
        public List<IndexTicketViewModel> AllSubsribedTickets { get; set; }
    }
}