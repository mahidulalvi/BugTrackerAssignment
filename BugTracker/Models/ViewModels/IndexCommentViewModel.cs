using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class IndexCommentViewModel
    {
        public string CommentId { get; set; }
        public string CommentData { get; set; }
        public string CommentOwnerId { get; set; }
        public string CommentOwnerName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }    
        public bool AvailableForUser { get; set; }
    }
}