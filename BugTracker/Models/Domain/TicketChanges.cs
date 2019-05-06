using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class TicketChanges
    {
        public string Id { get; set; }
        public string ChangedValue { get; set; }
        public string PreviousValue { get; set; }
        public string PropertyName { get; set; }
        public DateTime DateCreated { get; set; }


        public string TicketHistoryId { get; set; }
        public virtual TicketHistory TicketHistory { get; set; }

        public TicketChanges()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
        }
    }
}