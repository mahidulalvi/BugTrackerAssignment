using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Ticket
    {
        public string Id { get; set; }
        public string TicketTitle { get; set; }
        public string TicketDescription { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual List<TicketHistory> TicketHistories { get; set; }

        //public string MediaUrl { get; set; }
        public virtual List<FileClass> MediaUrls { get; set; }

        public virtual List<Comment> Comments { get; set; }        

        public virtual TicketType TicketType { get; set; }
        public string TicketTypeId { get; set; }

        public virtual TicketPriority TicketPriority { get; set; }
        public string TicketPriorityId { get; set; }

        public virtual TicketStatus TicketStatus { get; set; }
        public string TicketStatusId { get; set; }

        public virtual Project Project { get; set; }
        public string ProjectId { get; set; }


        public virtual List<ApplicationUser> AssignedMembers { get; set; }

        public virtual ApplicationUser TicketOwner { get; set; }
        public string TicketOwnerId { get; set; }

        public virtual List<ApplicationUser> SubscribedUsers { get; set; }

        public Ticket()
        {
            Id = Guid.NewGuid().ToString();
            AssignedMembers = new List<ApplicationUser>();
            MediaUrls = new List<FileClass>();
            SubscribedUsers = new List<ApplicationUser>();
        }
    }
}