using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class DetailsProjectViewModel
    {
        public string Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDetails { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int MemberCount { get; set; }
        public int TicketCount { get; set; }
        public string UserName { get; set; }
    }
}