using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BugTracker.Models.Helpers
{
    public class RolesAndUsersHelper
    {
        private UserManager<ApplicationUser> userManager;
        private ApplicationDbContext context;
        private RoleManager<IdentityRole> roleManager;

        public RolesAndUsersHelper(ApplicationDbContext db)
        {
            context = db;
            userManager = OwinContextExtensions.GetUserManager<ApplicationUserManager>(HttpContext.Current.GetOwinContext());            
            roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));
        }

        public List<string> AllRoles()
        {
            return roleManager.Roles.Select(p => p.Name).ToList();
        }


        public void AddRole(string roleName)
        {
            var role = new IdentityRole(roleName);
            roleManager.Create(role);
        }

        public List<string> AllUserIds()
        {
            var allUserIds = userManager.Users.Select(p => p.Id).ToList();

            return allUserIds;
        }

        public List<string> AllUserNames()
        {
            var allUserNames = userManager.Users.Select(p => p.UserName).ToList();

            return allUserNames;
        }

        public bool AddUserToProject(string projectId, string userId)
        {
            var project = context.Projects.FirstOrDefault(p => p.Id == projectId);
            var userToBeAdded = context.Users.FirstOrDefault(p => p.Id == userId);

            if (project == null || userToBeAdded == null)
            {
                return false;
            }

            project.Users.Add(userToBeAdded);
            userToBeAdded.Projects.Add(project);

            return true;   
        }


        public bool AssignUserToTicket (string ticketId, string userId)
        {
            var ticket = context.Tickets.FirstOrDefault(p => p.Id == ticketId);
            var userToBeAdded = context.Users.FirstOrDefault(p => p.Id == userId);

            if (ticket == null || userToBeAdded == null)
            {
                return false;
            }

            ticket.AssignedMembers.Add(userToBeAdded);
            userToBeAdded.AssignedTickets.Add(ticket);

            return true;
        }


        public bool RemoveUserFromProject(string projectId, string userId)
        {
            var project = context.Projects.FirstOrDefault(p => p.Id == projectId);
            var userToBeRemoved = context.Users.FirstOrDefault(p => p.Id == userId);

            if (project == null || userToBeRemoved == null)
            {
                return false;
            }

            project.Users.Remove(userToBeRemoved);
            userToBeRemoved.Projects.Remove(project);

            return true;
        }


        public bool UnassignUserFromTicket (string ticketId, string userId)
        {
            var ticket = context.Tickets.FirstOrDefault(p => p.Id == ticketId);
            var userToBeUnassigned = context.Users.FirstOrDefault(p => p.Id == userId);

            if (ticket == null || userToBeUnassigned == null)
            {
                return false;
            }

            ticket.AssignedMembers.Remove(userToBeUnassigned);
            userToBeUnassigned.AssignedTickets.Remove(ticket);

            return true;
        }


        public string GetRoleId(string roleName)
        {
            var role = context.Roles.FirstOrDefault(p => p.Name == roleName);

            if (role != null)
            {
                return role.Id;
            }
            else
            {
                return "";
            }
        }



        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }
        public ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }
        public List<string> ListUserRolesInListStringFormat(string userId)
        {
            return userManager.GetRoles(userId).ToList();
        }
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }
        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }
        public ICollection<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }
    }
}