using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ProjectMemberViewModel
    {
        public string Id { get; set; }
        public string NameOfUser { get; set; }
        public List<string> Roles { get; set; }
    }
}