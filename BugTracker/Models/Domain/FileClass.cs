using System;

namespace BugTracker.Models.Domain
{
    public class FileClass
    {
        public string Id { get; set; }
        public string MediaUrl { get; set; }

        public virtual Ticket Ticket { get; set; }
        public string TicketId { get; set; }

        public virtual ApplicationUser AttachmentOwner { get; set; }
        public string AttachmentOwnerId { get; set; }

        public FileClass()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}