using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class FullTicketHistory
    {
        public string UserName { get; set; }
        public DateTime DateUpdated { get; set; }
        public List<string> Changes { get; set; }

        public FullTicketHistory()
        {
            Changes = new List<string>();
        }
    }
}