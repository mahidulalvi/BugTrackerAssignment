using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ManageTicketMembersViewModel
    {
        public string TicketId { get; set; }
        public string TicketName { get; set; }
        public List<ProjectMemberViewModel> AllUsers { get; set; }
        public List<ProjectMemberViewModel> CurrentMembers { get; set; }

        public ManageTicketMembersViewModel()
        {
            AllUsers = new List<ProjectMemberViewModel>();
            CurrentMembers = new List<ProjectMemberViewModel>();
        }
    }
}