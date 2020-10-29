using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

            if (project == null || project.Archived == true || userToBeAdded == null)
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

            if (ticket == null || ticket.Project.Archived == true || userToBeAdded == null)
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

            if (project == null || project.Archived == true || userToBeRemoved == null)
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

            if (ticket == null || ticket.Project.Archived == true || userToBeUnassigned == null)
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

        [Authorize]
        public async Task<ActionResult> MassEmailSender(string ticketId, string type)
        {
            var ticket = context.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (ticket == null || ticket.Project.Archived == true)
            {
                return new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Tickets"},
                        { "Action", "AllTickets"}
                    });
            }

            if (type == "Modify")
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                var counter = 0;

                foreach (var user in ticket.AssignedMembers)
                {
                    if (timer.Elapsed.Seconds < 10 && counter >= 2)
                    {
                        await Task.Delay(10000 - timer.Elapsed.Milliseconds);
                        timer.Reset();
                        timer.Start();
                        counter = 0;
                        await SendEmail(user.Id, ticket.TicketTitle, "Modify");
                        counter += 1;
                    }
                    else if (counter < 2)
                    {
                        await SendEmail(user.Id, ticket.TicketTitle, "Modify");
                        counter += 1;
                    }
                }

                foreach (var user in ticket.SubscribedUsers)
                {
                    if (timer.Elapsed.Seconds < 10 && counter >= 2)
                    {
                        await Task.Delay(10000 - timer.Elapsed.Milliseconds);
                        timer.Reset();
                        timer.Start();
                        counter = 0;
                        await SendEmail(user.Id, ticket.TicketTitle, "Modify");
                        counter += 1;
                    }
                    else if (counter <= 2)
                    {
                        await SendEmail(user.Id, ticket.TicketTitle, "Modify");
                        counter += 1;
                    }
                }
            }

            return new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Tickets"},
                        { "Action", "AllTickets"}
                    });
        }

        [Authorize]
        public async Task<ActionResult> SendEmail(string userId, string ticketTitle, string operation)
        {
            var user = context.Users.FirstOrDefault(p => p.Id == userId);            

            if (user == null)
            {
                new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Tickets"},
                        { "Action", "AllTickets"}
                    });
            }

            if (operation == "Add")
            {
                await userManager.SendEmailAsync(userId, $"Assigned to Ticket {ticketTitle}", $"You have been added to ticket {ticketTitle}");
            }
            else if (operation == "Remove")
            {
                await userManager.SendEmailAsync(userId, $"Unassigned from Ticket {ticketTitle}", $"You have been unassigned from ticket {ticketTitle}");
            }
            else if (operation == "Modify")
            {
                await userManager.SendEmailAsync(userId, $"Ticket {ticketTitle} has been modified", $"An user just modified ticket {ticketTitle}");
            }
            else
            {
                new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Tickets"},
                        { "Action", "AllTickets"}
                    });
            }

            return new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Tickets"},
                        { "Action", "AllTickets"}
                    });
        }
    }
}