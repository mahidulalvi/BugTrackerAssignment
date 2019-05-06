using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class TicketHistory
    {
        public string Id { get; set; }
        public DateTime DateUpdated { get; set; }

        public string TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual List<TicketChanges> Changes { get; set; }

        public TicketHistory()
        {
            Id = Guid.NewGuid().ToString();
            Changes = new List<TicketChanges>();
        }
    }
}