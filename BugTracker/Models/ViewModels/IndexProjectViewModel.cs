using System;

namespace BugTracker.Models.ViewModels
{
    public class IndexProjectViewModel
    {
        public string Id { get; set; }
        public string ProjectName { get; set; }        
        public int MemberCount { get; set; }
        public int TicketCount { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UserName { get; set; }
    }
}