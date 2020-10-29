using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<IndexProjectViewModel> AllProjects { get; set; }
        public int TotalProjects { get; set; }
        public List<IndexTicketViewModel> AllTickets { get; set; }
        public int TotalTickets { get; set; }

        public DashboardViewModel()
        {
            AllProjects = new List<IndexProjectViewModel>();
            AllTickets = new List<IndexTicketViewModel>();
        }
    }
}