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

        // GET: Main




        [Authorize]
        public ActionResult Index()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                return RedirectToAction(nameof(MainController.CurrentUserIndex));
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




        [Authorize]
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




        //[HttpGet]
        //[Authorize]
        //public ActionResult AllTickets()
        //{
        //    string currentUserId = User.Identity.GetUserId();
        //    List<IndexTicketViewModel> viewModel;
        //    if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //    {
        //        viewModel = DbContext.Tickets
        //            .Select(
        //            ticket => new IndexTicketViewModel
        //            {
        //                Id = ticket.Id,
        //                TicketTitle = ticket.TicketTitle,
        //                TicketDescription = ticket.TicketDescription,
        //                TicketType = ticket.TicketType.TypeName,
        //                TicketPriority = ticket.TicketPriority.PriorityLevel,
        //                TicketStatus = ticket.TicketStatus.StatusName,
        //                DateCreated = ticket.DateCreated,
        //                DateUpdated = ticket.DateUpdated,
        //                TicketOwner = new ProjectMemberViewModel()
        //                {
        //                    Id = ticket.TicketOwnerId,
        //                    NameOfUser = ticket.TicketOwner.NameOfUser,
        //                    Roles = (from userRoles in ticket.TicketOwner.Roles
        //                             join roles in DbContext.Roles
        //                             on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                },
        //                TicketAssignees = ticket.AssignedMembers
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                }).ToList(),
        //                TicketNonAssignees = DbContext.Users
        //                .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                })
        //                .ToList(),
        //                ProjectId = ticket.ProjectId,
        //                ProjectName = ticket.Project.ProjectName
        //            }).ToList();
        //    }
        //    else if (User.IsInRole("Developer") && User.IsInRole("Submitter"))
        //    {
        //        viewModel = DbContext.Tickets
        //        .Where(p => ((p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId != currentUserId) || /*2*/ (p.Project.Users.Any(r => r.Id == currentUserId) && !p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*3*/ (p.TicketOwnerId == currentUserId && p.AssignedMembers.Any(r => r.Id == currentUserId) && !p.Project.Users.Any(r => r.Id == currentUserId)) || /*4*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.AssignedMembers.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId)/*5*/))
        //        .Select(
        //            ticket => new IndexTicketViewModel
        //            {
        //                Id = ticket.Id,
        //                TicketTitle = ticket.TicketTitle,
        //                TicketDescription = ticket.TicketDescription,
        //                TicketType = ticket.TicketType.TypeName,
        //                TicketPriority = ticket.TicketPriority.PriorityLevel,
        //                TicketStatus = ticket.TicketStatus.StatusName,
        //                DateCreated = ticket.DateCreated,
        //                DateUpdated = ticket.DateUpdated,
        //                TicketOwner = new ProjectMemberViewModel()
        //                {
        //                    Id = ticket.TicketOwnerId,
        //                    NameOfUser = ticket.TicketOwner.NameOfUser,
        //                    Roles = (from userRoles in ticket.TicketOwner.Roles
        //                             join roles in DbContext.Roles
        //                             on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                },
        //                TicketAssignees = ticket.AssignedMembers
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                }).ToList(),
        //                TicketNonAssignees = DbContext.Users
        //                .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                })
        //                .ToList(),
        //                ProjectId = ticket.ProjectId,
        //                ProjectName = ticket.Project.ProjectName
        //            }).ToList();
        //    }
        //    else if (User.IsInRole("Developer"))
        //    {
        //        viewModel = DbContext.Tickets
        //        .Where(p => (p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId)) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.AssignedMembers.Any(r => r.Id == currentUserId)))
        //        .Select(
        //            ticket => new IndexTicketViewModel
        //            {
        //                Id = ticket.Id,
        //                TicketTitle = ticket.TicketTitle,
        //                TicketDescription = ticket.TicketDescription,
        //                TicketType = ticket.TicketType.TypeName,
        //                TicketPriority = ticket.TicketPriority.PriorityLevel,
        //                TicketStatus = ticket.TicketStatus.StatusName,
        //                DateCreated = ticket.DateCreated,
        //                DateUpdated = ticket.DateUpdated,
        //                TicketOwner = new ProjectMemberViewModel()
        //                {
        //                    Id = ticket.TicketOwnerId,
        //                    NameOfUser = ticket.TicketOwner.NameOfUser,
        //                    Roles = (from userRoles in ticket.TicketOwner.Roles
        //                             join roles in DbContext.Roles
        //                             on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                },
        //                TicketAssignees = ticket.AssignedMembers
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                }).ToList(),
        //                TicketNonAssignees = DbContext.Users
        //                .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                })
        //                .ToList(),
        //                ProjectId = ticket.ProjectId,
        //                ProjectName = ticket.Project.ProjectName
        //            }).ToList();
        //    }
        //    else if (User.IsInRole("Submitter"))
        //    {
        //        viewModel = DbContext.Tickets
        //        .Where(p => (p.Project.Users.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId))
        //        .Select(
        //            ticket => new IndexTicketViewModel
        //            {
        //                Id = ticket.Id,
        //                TicketTitle = ticket.TicketTitle,
        //                TicketDescription = ticket.TicketDescription,
        //                TicketType = ticket.TicketType.TypeName,
        //                TicketPriority = ticket.TicketPriority.PriorityLevel,
        //                TicketStatus = ticket.TicketStatus.StatusName,
        //                DateCreated = ticket.DateCreated,
        //                DateUpdated = ticket.DateUpdated,
        //                TicketOwner = new ProjectMemberViewModel()
        //                {
        //                    Id = ticket.TicketOwnerId,
        //                    NameOfUser = ticket.TicketOwner.NameOfUser,
        //                    Roles = (from userRoles in ticket.TicketOwner.Roles
        //                             join roles in DbContext.Roles
        //                             on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                },
        //                TicketAssignees = ticket.AssignedMembers
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                }).ToList(),
        //                TicketNonAssignees = DbContext.Users
        //                .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                })
        //                .ToList(),
        //                ProjectId = ticket.ProjectId,
        //                ProjectName = ticket.Project.ProjectName
        //            }).ToList();
        //    }
        //    else
        //    {
        //        viewModel = null;
        //    }

        //    var a = DbContext.Tickets.First();
        //    var b = DbContext.Projects.ToList();

        //    return View(viewModel);
        //}





        //[HttpGet]
        //[Authorize]
        //public ActionResult ViewTicket(string id)
        //{
        //    var currentUserId = User.Identity.GetUserId();
        //    var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);

        //    var model = new IndexTicketViewModel();

        //    model.Id = ticket.Id;
        //    model.TicketTitle = ticket.TicketTitle;
        //    model.TicketDescription = ticket.TicketDescription;
        //    model.TicketType = ticket.TicketType.TypeName;
        //    model.TicketPriority = ticket.TicketPriority.PriorityLevel;
        //    model.TicketStatus = ticket.TicketStatus.StatusName;
        //    model.DateCreated = ticket.DateCreated;
        //    model.DateUpdated = ticket.DateUpdated;
        //    model.TicketOwner = new ProjectMemberViewModel()
        //    {
        //        Id = ticket.TicketOwnerId,
        //        NameOfUser = ticket.TicketOwner.NameOfUser,
        //        Roles = (from userRoles in ticket.TicketOwner.Roles
        //                 join roles in DbContext.Roles
        //                 on userRoles.RoleId equals roles.Id
        //                 select roles.Name).ToList()
        //    };
        //    model.TicketOwner.Id = ticket.TicketOwnerId;
        //    model.TicketOwner.NameOfUser = ticket.TicketOwner.NameOfUser;
        //    model.TicketOwner.Roles = (from userRoles in ticket.TicketOwner.Roles
        //                               join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                               select roles.Name).ToList();
        //    model.TicketAssignees = ticket.AssignedMembers
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                }).ToList();
        //    model.TicketNonAssignees = DbContext.Users
        //                .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
        //                .Select(selectuser => new ProjectMemberViewModel
        //                {
        //                    Id = selectuser.Id,
        //                    NameOfUser = selectuser.NameOfUser,
        //                    Roles = (from userRoles in selectuser.Roles
        //                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                             select roles.Name).ToList()
        //                })
        //                .ToList();
        //    model.ProjectId = ticket.ProjectId;
        //    model.ProjectName = ticket.Project.ProjectName;

        //    foreach (var image in ticket.MediaUrls)
        //    {
        //        model.MediaUrls.Add(image.MediaUrl);
        //    }

        //    foreach (var comment in ticket.Comments)
        //    {
        //        model.Comments.Add(comment.CommentData);
        //    }

        //    if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //    {
        //        model.AvailableForUser = true;
        //    }
        //    else if (User.IsInRole("Developer") && ticket.AssignedMembers.Any(p => p.Id == currentUserId))
        //    {
        //        model.AvailableForUser = true;
        //    }
        //    else if (User.IsInRole("Submitter") && ticket.TicketOwnerId == currentUserId)
        //    {
        //        model.AvailableForUser = true;
        //    }
        //    else
        //    {
        //        model.AvailableForUser = false;
        //    }

        //    return View(model);
        //}



        //[HttpPost]
        //[Authorize]
        //public ActionResult CreateComment(IndexTicketViewModel formdata, string ticketId)
        //{
        //    var currentUserId = User.Identity.GetUserId();
        //    var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
        //    var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

        //    if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
        //    {
        //        if (User.IsInRole("Developer") && !ticket.AssignedMembers.Any(p => p.Id == currentUserId))
        //        {
        //            return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
        //        }
        //        if (User.IsInRole("Submitter") && ticket.TicketOwnerId != currentUserId)
        //        {
        //            return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
        //        }
        //    }


        //    var comment = new Comment();
        //    comment.CommentCreatorId = currentUserId;
        //    comment.TicketId = ticketId;
        //    comment.CommentData = formdata.CommentData;

        //    ticket.Comments.Add(comment);
        //    currentUser.Comments.Add(comment);

        //    DbContext.Comments.Add(comment);

        //    DbContext.SaveChanges();

        //    return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
        //}





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





        //[HttpGet]
        //[Authorize(Roles = "Admin, Project Manager")]
        //public ActionResult ManageTicketMembers(string ticketId)
        //{
        //    if (ticketId == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }

        //    var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

        //    if (ticket == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }

        //    var model = new ManageTicketMembersViewModel();

        //    model.TicketName = ticket.TicketTitle;
        //    model.TicketId = ticket.Id;

        //    model.CurrentMembers = ticket
        //        .AssignedMembers.Select(selectuser => new ProjectMemberViewModel
        //        {
        //            Id = selectuser.Id,
        //            NameOfUser = selectuser.NameOfUser,
        //            Roles = (from userRoles in selectuser.Roles
        //                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                     select roles.Name).ToList()
        //        }).ToList();

        //    var idOfDeveloperRole = RolesAndUsersHelper.GetRoleId("Developer");

        //    model.AllUsers = DbContext.Users
        //        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticketId) && p.Roles.Any(r => r.RoleId == idOfDeveloperRole))
        //        .Select(selectuser => new ProjectMemberViewModel
        //        {
        //            Id = selectuser.Id,
        //            NameOfUser = selectuser.NameOfUser,
        //            Roles = (from userRoles in selectuser.Roles
        //                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
        //                     select roles.Name).ToList()
        //        })
        //        .ToList();

        //    return View(model);
        //}





        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ToggleRoleByYesOrNo(string id, string roleName)
        {

            if (id == null || roleName == null)
            {
                var errorMessage = "Sorry! Something went wrong";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }

            if (id == User.Identity.GetUserId() && roleName == "Admin")
            {
                var errorMessage = "Admins cannot resign from their own role as admin.";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.AdminsDesk));
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
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }
            else
            {
                var errorMessage = "Encountered an error! Try again";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }
        }






        //[HttpPost]
        //[Authorize(Roles = "Admin, Project Manager")]
        //public ActionResult ToggleTicketDeveloper(string ticketId, string userId, string operation)
        //{
        //    if (ticketId == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }

        //    if (userId == null || operation == null)
        //    {
        //        return RedirectToAction(nameof(MainController.ManageTicketMembers), new { ticketId = ticketId });
        //    }

        //    bool decision;

        //    if (operation == "Add")
        //    {
        //        decision = RolesAndUsersHelper.AssignUserToTicket(ticketId, userId);
        //    }
        //    else if (operation == "Remove")
        //    {
        //        decision = RolesAndUsersHelper.UnassignUserFromTicket(ticketId, userId);
        //    }
        //    else
        //    {
        //        decision = false;
        //    }

        //    DbContext.SaveChanges();

        //    if (decision == true)
        //    {
        //        return RedirectToAction(nameof(MainController.ManageTicketMembers), new { ticketId = ticketId });
        //    }
        //    else
        //    {
        //        var errorMessage = "Encountered an error!";
        //        TempData["errorMessage"] = errorMessage;
        //        return RedirectToAction(nameof(MainController.ManageTicketMembers), new { ticketId = ticketId });
        //    }
        //}






        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ClearAllRoles(string id)
        {

            if (id == null)
            {
                var errorMessage = "Sorry! Something went wrong";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }

            if (id == User.Identity.GetUserId())
            {
                var errorMessage = "Admins cannot resign from their own role as admin.";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }

            bool decision = false;


            var rolesOfUser = RolesAndUsersHelper.ListUserRoles(id);

            if (rolesOfUser.Count < 1)
            {
                decision = true;
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }
            foreach (var role in rolesOfUser)
            {
                decision = RolesAndUsersHelper.RemoveUserFromRole(id, role);
            }

            DbContext.SaveChanges();

            if (decision == true)
            {
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }
            else
            {
                var errorMessage = "Encountered an error! Try again";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.AdminsDesk));
            }
        }






        [HttpGet]
        [Authorize]
        public ActionResult DetailsOfProject(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(MainController.Index));
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
                return RedirectToAction(nameof(MainController.Index));
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
                    return RedirectToAction(nameof(MainController.Index));
                }
                project.DateUpdated = DateTime.Now;
            }

            project.ProjectName = formdata.ProjectName;
            project.ProjectDetails = formdata.ProjectDetails;
            DbContext.SaveChanges();

            return RedirectToAction(nameof(MainController.Index));
        }






        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult EditAProject(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction(nameof(MainController.Index));
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
                return RedirectToAction(nameof(MainController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            SaveProject(id, formdata);

            return RedirectToAction(nameof(MainController.Index));
        }





        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult DeleteAProject(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var CorrespondingTickets = DbContext.Tickets
                .Where(p => p.ProjectId == id).ToList();
            CorrespondingTickets.Clear();

            DbContext.Projects.Remove(project);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(MainController.Index));
        }





        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ManageMembers(string projectId)
        {
            if (projectId == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                return RedirectToAction(nameof(MainController.Index));
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
                return RedirectToAction(nameof(MainController.Index));
            }

            if (userId == null || operation == null)
            {
                return RedirectToAction(nameof(MainController.ManageMembers), new { projectId = projectId });
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
                return RedirectToAction(nameof(MainController.ManageMembers), new { projectId = projectId });
            }
            else
            {
                var errorMessage = "Encountered an error!";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(MainController.ManageMembers), new { projectId = projectId });
            }
        }





        //[HttpGet]
        //[Authorize(Roles = "Submitter")]
        //public ActionResult _CreateTicket(string projectId)
        //{
        //    var currentUserId = User.Identity.GetUserId();



        //    if (projectId == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }


        //    var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId && p.Users.Any(r => r.Id == currentUserId));

        //    if (project == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }

        //    var model = new CreateEditTicketViewModel();
        //    model.ProjectId = projectId;
        //    ViewBag.DropDownListForPriorities = DbContext.TicketPriorities.Select(p => p.PriorityLevel).ToList();
        //    ViewBag.DropDownListForTypes = DbContext.TicketTypes.Select(p => p.TypeName).ToList();
        //    return View(model);
        //}






        //[HttpPost]
        //[Authorize(Roles = "Submitter")]
        //public ActionResult _CreateTicket(CreateEditTicketViewModel formdata)
        //{
        //    if (formdata == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }

        //    return SaveTicket(null, formdata);
        //}


        //[HttpGet]
        //[Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        //public ActionResult _EditTicket(string ticketId)
        //{
        //    var currentUserId = User.Identity.GetUserId();
        //    var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
        //    var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

        //    if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
        //    {
        //        if (User.IsInRole("Developer") && !ticket.AssignedMembers.Any(p => p.Id == currentUserId))
        //        {
        //            return RedirectToAction(nameof(MainController.AllTickets));
        //        }
        //        else if (User.IsInRole("Submitter") && ticket.TicketOwnerId != currentUserId)
        //        {
        //            return RedirectToAction(nameof(MainController.AllTickets));
        //        }
        //    }


        //    var model = new CreateEditTicketViewModel();
        //    model.TicketId = ticket.Id;
        //    model.TicketTitle = ticket.TicketTitle;
        //    model.TicketDescription = ticket.TicketDescription;
        //    model.TicketType = ticket.TicketType.TypeName;
        //    model.TicketPriority = ticket.TicketPriority.PriorityLevel;
        //    foreach (var image in ticket.MediaUrls)
        //    {
        //        model.AllUploadedFiles.Add(image.MediaUrl);
        //    }

        //    if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //    {
        //        model.AllProjectNames = currentUser.Projects.Select(p => p.ProjectName).ToList();
        //    }
        //    else
        //    {
        //        model.AllProjectNames = currentUser.Projects.Where(p => p.Users.Any(r => r.Id == currentUserId)).Select(t => t.ProjectName).ToList();
        //    }

        //    if (model.AllProjectNames.Any())
        //    {
        //        model.ProjectName = ticket.Project.ProjectName;
        //    }

        //    if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //    {
        //        model.TicketStatus = ticket.TicketStatus.StatusName;
        //        model.DropDownForStatuses = DbContext.TicketStatuses.Select(p => p.StatusName).ToList();
        //    }

        //    model.DropDownForPriorities = DbContext.TicketPriorities.Select(p => p.PriorityLevel).ToList();
        //    model.DropDownForTypes = DbContext.TicketTypes.Select(p => p.TypeName).ToList();

        //    return View(model);
        //}




        //[HttpPost]
        //[Authorize]
        //public ActionResult _EditTicket(CreateEditTicketViewModel formdata)
        //{
        //    if (formdata == null)
        //    {
        //        return RedirectToAction(nameof(MainController.Index));
        //    }
        //    var currentUserId = User.Identity.GetUserId();
        //    var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == formdata.TicketId);

        //    if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
        //    {
        //        if (User.IsInRole("Developer") && !ticket.AssignedMembers.Any(p => p.Id == currentUserId))
        //        {
        //            return RedirectToAction(nameof(MainController.AllTickets));
        //        }
        //        else if (User.IsInRole("Submitter") && ticket.TicketOwnerId != currentUserId)
        //        {
        //            return RedirectToAction(nameof(MainController.AllTickets));
        //        }
        //    }
        //    return SaveTicket(formdata.TicketId, formdata);
        //}






        //private ActionResult SaveTicket(string id, CreateEditTicketViewModel formdata)
        //{
        //    var userId = User.Identity.GetUserId();
        //    var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

        //    var ticketType = DbContext.TicketTypes.FirstOrDefault(p => p.TypeName == formdata.TicketType);
        //    var ticketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.PriorityLevel == formdata.TicketPriority);

        //    string fileExtension;

        //    //Validating file upload
        //    if (formdata.Media != null)
        //    {
        //        for (var i = 0; i < formdata.Media.Count(); i++)
        //        {
        //            fileExtension = Path.GetExtension(formdata.Media[i].FileName);

        //            if (!Constants.AllowedFileExtensions.Contains(fileExtension))
        //            {
        //                ModelState.AddModelError("", "File extension is not allowed.");

        //                return RedirectToAction(nameof(MainController.AllTickets));
        //            }
        //        }
        //    }



        //    Ticket ticket;

        //    if (id == null)
        //    {
        //        ticket = new Ticket();
        //        ticket.DateCreated = DateTime.Now;
        //        var ticketStatus = DbContext.TicketStatuses.FirstOrDefault(p => p.StatusName == "Open");

        //        ticket.TicketTypeId = ticketType.Id;
        //        ticket.TicketPriorityId = ticketPriority.Id;
        //        ticket.TicketStatusId = ticketStatus.Id;

        //        ticketType.Tickets.Add(ticket);
        //        ticketPriority.Tickets.Add(ticket);
        //        ticketStatus.Tickets.Add(ticket);

        //        var project = DbContext.Projects.FirstOrDefault(p => p.Id == formdata.ProjectId);

        //        ticket.ProjectId = formdata.ProjectId;
        //        project.Tickets.Add(ticket);

        //        ticket.TicketOwnerId = currentUser.Id;
        //        currentUser.OwnedTickets.Add(ticket);

        //        //ticket.AssignedMembers.Add(currentUser);
        //        //currentUser.AssignedTickets.Add(ticket);

        //        DbContext.Tickets.Add(ticket);
        //    }
        //    else
        //    {
        //        ticket = DbContext.Tickets.FirstOrDefault(
        //            p => p.Id == id);

        //        if (ticket == null)
        //        {
        //            return RedirectToAction(nameof(MainController.Index));
        //        }

        //        bool developerIsAssigned = ticket.AssignedMembers.Any(p => p.Id == userId);
        //        bool submitterOwns = ticket.TicketOwnerId == userId;
        //        bool userValidForThisCondition = (User.IsInRole("Developer") || User.IsInRole("Submitter"));

        //        if (userValidForThisCondition && developerIsAssigned == false && submitterOwns == false)
        //        {
        //            return RedirectToAction(nameof(Index));
        //        }

        //        ticket.DateUpdated = DateTime.Now;

        //        ticket.TicketTypeId = ticketType.Id;
        //        ticket.TicketPriorityId = ticketPriority.Id;

        //        if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
        //        {
        //            ticket.TicketStatusId = DbContext.TicketStatuses.FirstOrDefault(p => p.StatusName == formdata.TicketStatus).Id;
        //        }


        //        if (formdata.ProjectName != ticket.Project.ProjectName)
        //        {
        //            var previousProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);
        //            previousProject.Tickets.Remove(ticket);

        //            ticket.ProjectId = DbContext.Projects.Where(p => p.Users.Any(r => r.Id == userId)/*) && */).FirstOrDefault(p => p.ProjectName == formdata.ProjectName).Id;

        //            var currentProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);

        //            currentProject.Tickets.Add(ticket);
        //        }
        //    }


        //    //Handling file upload
        //    if (formdata.Media != null)
        //    {
        //        if (!Directory.Exists(Constants.MappedUploadFolder))
        //        {
        //            Directory.CreateDirectory(Constants.MappedUploadFolder);
        //        }

        //        for(var i = 0; i < formdata.Media.Count(); i++)
        //        {
        //            var fileName = formdata.Media[i].FileName;
        //            var fullPathWithName = Constants.MappedUploadFolder + fileName;
        //            formdata.Media[i].SaveAs(fullPathWithName);
        //            var newImage = new FileClass();
        //            newImage.MediaUrl = Constants.UploadFolder + fileName;
        //            newImage.TicketId = ticket.Id;
        //            ticket.MediaUrls.Add(newImage);
        //            DbContext.FileClasses.Add(newImage);
        //        }
        //    };


        //    ticket.TicketTitle = formdata.TicketTitle;
        //    ticket.TicketDescription = formdata.TicketDescription;

        //    DbContext.SaveChanges();

        //    return RedirectToAction(nameof(MainController.Index));
        //}
    }
}