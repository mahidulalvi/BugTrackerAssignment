using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Project
    {
        public string Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDetails { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }

        public virtual List<Ticket> Tickets { get; set; }

        public int TicketCount
        {
            get
            {
                return Tickets.Count();
            }
        }

        public Project()
        {
            Id = Guid.NewGuid().ToString();
            Users = new List<ApplicationUser>();
            Tickets = new List<Ticket>();
            DateCreated = DateTime.Now;
        }
    }
}