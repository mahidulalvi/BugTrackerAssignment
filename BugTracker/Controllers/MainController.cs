using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace BugTracker.Controllers
{
    public class MainController : Controller
    {
        private ApplicationDbContext DbContext;
        private RolesAndUsersHelper RolesAndUsersHelper;


        public MainController()
        {
            DbContext = new ApplicationDbContext();
            RolesAndUsersHelper = new RolesAndUsersHelper(DbContext);
        }


        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                return RedirectToAction("CurrentUserIndex", "Main");
            }

            var viewModel = DbContext.Projects
                   .Select(
                   project => new IndexProjectViewModel
                   {
                       Id = project.Id,
                       ProjectName = project.ProjectName,
                       MemberCount = project.Users.Count(),
                       TicketCount = project.Tickets.Count(),
                       DateCreated = project.DateCreated,
                       DateUpdated = project.DateUpdated,
                       UserName = project.User.UserName
                   }).ToList();

            return View(viewModel);
        }


        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult CurrentUserIndex()
        {
            var userId = User.Identity.GetUserId();

            var viewModel = DbContext.Projects
                   .Where(p => p.Users.Any(r => r.Id == userId))
                   .Select(
                   project => new IndexProjectViewModel
                   {
                       Id = project.Id,
                       ProjectName = project.ProjectName,
                       MemberCount = project.Users.Count(),
                       TicketCount = project.Tickets.Count(),
                       DateCreated = project.DateCreated,
                       DateUpdated = project.DateUpdated,
                       UserName = project.User.UserName
                   }).ToList();

            return View("Index", viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AdminsDesk()
        {
            var rolesAndUsersHelper = new RolesAndUsersHelper(DbContext);

            var allRoles = rolesAndUsersHelper.AllRoles();

            var model = DbContext.Users.Select(p => new ObjectOfAdminsDeskNecessities
            {
                ProvidedUserId = p.Id,
                ProvidedUserName = p.UserName,
                ProvidedRoles = (from userRoles in p.Roles
                                 join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                 select roles.Name).ToList(),
                AllRoles = (from roles in DbContext.Roles select roles.Name).ToList()
            }).ToList();

            if (TempData["errorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["errorMessage"];
            }

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ToggleRoleByYesOrNo(string id, string roleName)
        {

            if (id == null || roleName == null)
            {
                var errorMessage = "Sorry! Something went wrong";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("AdminsDesk", "Main");
            }

            if (id == User.Identity.GetUserId() && roleName == "Admin")
            {
                var errorMessage = "Admins cannot resign from their own role as admin.";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("AdminsDesk", "Main");
            }

            bool decision;

            if (RolesAndUsersHelper.IsUserInRole(id, roleName))
            {
                decision = RolesAndUsersHelper.RemoveUserFromRole(id, roleName);
            }
            else
            {
                decision = RolesAndUsersHelper.AddUserToRole(id, roleName);
            }

            DbContext.SaveChanges();

            if (decision == true)
            {
                return RedirectToAction("AdminsDesk", "Main");
            }
            else
            {
                var errorMessage = "Encountered an error! Try again";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("AdminsDesk", "Main");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ClearAllRoles(string id)
        {

            if (id == null)
            {
                var errorMessage = "Sorry! Something went wrong";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("AdminsDesk", "Main");
            }

            if (id == User.Identity.GetUserId())
            {
                var errorMessage = "Admins cannot resign from their own role as admin.";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("AdminsDesk", "Main");
            }

            bool decision = false;


            var rolesOfUser = RolesAndUsersHelper.ListUserRoles(id);

            if (rolesOfUser.Count < 1)
            {
                decision = true;
                return RedirectToAction("AdminsDesk", "Main");
            }
            foreach (var role in rolesOfUser)
            {
                decision = RolesAndUsersHelper.RemoveUserFromRole(id, role);
            }

            DbContext.SaveChanges();

            if (decision == true)
            {
                return RedirectToAction("AdminsDesk", "Main");
            }
            else
            {
                var errorMessage = "Encountered an error! Try again";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("AdminsDesk", "Main");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult DetailsOfProject(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var currentUserId = User.Identity.GetUserId();
            bool specialCase;
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                specialCase = true;
            }
            else
            {
                specialCase = false;
            }
            var project = DbContext.Projects
                .FirstOrDefault(
                p => p.Id == id && (p.Users.Any(r => r.Id == currentUserId) || specialCase == true));

            if (project == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var model = new DetailsProjectViewModel();
            model.Id = project.Id;
            model.ProjectName = project.ProjectName;
            model.ProjectDetails = project.ProjectDetails;
            model.MemberCount = project.Users.Count();
            model.TicketCount = project.TicketCount;
            model.DateCreated = project.DateCreated;
            model.DateUpdated = project.DateUpdated;
            model.UserName = project.User.UserName;

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult CreateAProject()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult CreateAProject(CreateEditProjectViewModel formdata)
        {
            return SaveProject(null, formdata);
        }


        private ActionResult SaveProject(string id, CreateEditProjectViewModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            Project project;

            if (id == null)
            {
                project = new Project();
                project.UserId = userId;
                project.Users.Add(currentUser);

                currentUser.Projects.Add(project);

                DbContext.Projects.Add(project);
            }
            else
            {
                project = DbContext.Projects.FirstOrDefault(
                    p => p.Id == id);

                if (project == null)
                {
                    return RedirectToAction("Index", "Main");
                }
                project.DateUpdated = DateTime.Now;
            }

            project.ProjectName = formdata.ProjectName;
            project.ProjectDetails = formdata.ProjectDetails;
            DbContext.SaveChanges();

            return RedirectToAction("Index", "Main");
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditAProject(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var model = new CreateEditProjectViewModel();
            model.ProjectName = project.ProjectName;
            model.ProjectDetails = project.ProjectDetails;
            model.Id = project.Id;

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditAProject(string id, CreateEditProjectViewModel formdata)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Main");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            SaveProject(id, formdata);

            return RedirectToAction("Index", "Main");
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult DeleteAProject(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var CorrespondingTickets = DbContext.Tickets
                .Where(p => p.ProjectId == id).ToList();
            CorrespondingTickets.Clear();

            DbContext.Projects.Remove(project);
            DbContext.SaveChanges();

            return RedirectToAction("Index", "Main");
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ManageMembers(string projectId)
        {
            if (projectId == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var model = new ManageProjectMembersViewModel();

            model.ProjectName = project.ProjectName;
            model.ProjectId = project.Id;

            model.CurrentMembers = project
                .Users.Select(selectuser => new ProjectMemberViewModel
                {
                    Id = selectuser.Id,
                    NameOfUser = selectuser.NameOfUser,
                    Roles = (from userRoles in selectuser.Roles
                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                             select roles.Name).ToList()
                }).ToList();




            model.AllUsers = DbContext.Users
                .Where(p => !p.Projects.Any(r => r.Id == projectId))
                .Select(selectuser => new ProjectMemberViewModel
                {
                    Id = selectuser.Id,
                    NameOfUser = selectuser.NameOfUser,
                    Roles = (from userRoles in selectuser.Roles
                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                             select roles.Name).ToList()
                })
                .ToList();

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ToggleUsersInProject(string projectId, string userId, string operation)
        {
            if (projectId == null)
            {
                return RedirectToAction("Index", "Main");
            }

            if (userId == null || operation == null)
            {
                return RedirectToAction("ManageMembers", "Main", new { projectId = projectId });
            }

            bool decision;

            if (operation == "Add")
            {
                decision = RolesAndUsersHelper.AddUserToProject(projectId, userId);
            }
            else if (operation == "Remove")
            {
                decision = RolesAndUsersHelper.RemoveUserFromProject(projectId, userId);
            }
            else
            {
                decision = false;
            }

            DbContext.SaveChanges();

            if (decision == true)
            {
                return RedirectToAction("ManageMembers", "Main", new { projectId = projectId });
            }
            else
            {
                var errorMessage = "Encountered an error!";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("ManageMembers", "Main", new { projectId = projectId });
            }
        }                

    }
}