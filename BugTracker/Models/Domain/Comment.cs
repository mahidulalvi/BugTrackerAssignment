using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Domain
{
    public class Comment
    {
        public string Id { get; set; }
        public string CommentData { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public virtual Ticket Ticket { get; set; }
        public string TicketId { get; set; }

        public virtual ApplicationUser CommentCreator { get; set; }
        public string CommentCreatorId { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
        }
    }
}