using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class CreateEditCommentViewModel
    {        
        public string CommentData { get; set; } 
        public string CommentId { get; set; }
        public string TicketId { get; set; }        
    }
}