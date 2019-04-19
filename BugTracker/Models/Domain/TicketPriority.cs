using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class TicketPriority
    {
        public string Id { get; set; }
        public string PriorityLevel { get; set; }

        public virtual List<Ticket> Tickets { get; set; }

        public TicketPriority()
        {
            Id = Guid.NewGuid().ToString();
            Tickets = new List<Ticket>();
        }
    }
}