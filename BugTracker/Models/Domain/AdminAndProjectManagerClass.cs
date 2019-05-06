using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class AdminAndProjectManagerClass
    {
        public string Id { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual List<Ticket> SubscribedTickets { get; set; }

        public AdminAndProjectManagerClass(string userId)
        {
            Id = Guid.NewGuid().ToString();
            SubscribedTickets = new List<Ticket>();
            UserId = userId;
        }
    }
}