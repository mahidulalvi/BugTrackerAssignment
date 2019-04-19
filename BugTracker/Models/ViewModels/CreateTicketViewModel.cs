using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class CreateEditTicketViewModel
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TicketId { get; set; }
        public string TicketTitle { get; set; }
        public string TicketDescription { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string TicketType { get; set; }
        public string TicketPriority { get; set; }
        public string TicketStatus { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public List<string> AllProjectNames { get; set; }
        public List<string> DropDownForTypes { get; set; }
        public List<string> DropDownForPriorities { get; set; }
        public List<string> DropDownForStatuses { get; set; }
        public List<string> AllUploadedFiles { get; set; }

        public List<HttpPostedFileBase> Media { get; set; }

        public CreateEditTicketViewModel()
        {
            AllUploadedFiles = new List<string>();
        }
    }
}