using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class CreateEditTicketViewModel
    {
        public string ProjectId { get; set; }
        [Required(ErrorMessage = "Project Required")]
        public string ProjectName { get; set; }
        public string TicketId { get; set; }
        [Required(ErrorMessage = "Title required")]
        public string TicketTitle { get; set; }
        [Required(ErrorMessage = "Description required")]
        public string TicketDescription { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        [Required(ErrorMessage = "Type required")]
        public string TicketType { get; set; }
        [Required(ErrorMessage = "Priority required")]
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