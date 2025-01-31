﻿using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class ManageProjectMembersViewModel
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<ProjectMemberViewModel> AllUsers { get; set; }
        public List<ProjectMemberViewModel> CurrentMembers { get; set; }

        public ManageProjectMembersViewModel()
        {
            AllUsers = new List<ProjectMemberViewModel>();
            CurrentMembers = new List<ProjectMemberViewModel>();
        }
    }
}