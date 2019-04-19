using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class TicketStatus
    {
        public string Id { get; set; }
        public string StatusName { get; set; }

        public virtual List<Ticket> Tickets { get; set; }

        public TicketStatus()
        {
            Id = Guid.NewGuid().ToString();
            Tickets = new List<Ticket>();
        }
    }
}