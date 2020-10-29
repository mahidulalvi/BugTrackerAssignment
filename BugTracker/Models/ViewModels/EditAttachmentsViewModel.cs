using System.Collections.Generic;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class EditAttachmentsViewModel
    {
        public string TicketId { get; set; }
        public List<string> MediaUrls { get; set; }
        public List<HttpPostedFileBase> Files { get; set; }
        public bool AvailableForUser { get; set; }

        public EditAttachmentsViewModel()
        {
            Files = new List<HttpPostedFileBase>();
        }
    }
}