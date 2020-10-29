using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class ProjectMemberViewModel
    {
        public string Id { get; set; }
        public string NameOfUser { get; set; }
        public List<string> Roles { get; set; }
    }
}