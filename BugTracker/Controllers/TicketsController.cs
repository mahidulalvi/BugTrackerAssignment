using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext DbContext;
        private RolesAndUsersHelper RolesAndUsersHelper;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public TicketsController()
        {
            DbContext = new ApplicationDbContext();
            RolesAndUsersHelper = new RolesAndUsersHelper(DbContext);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AllTickets()
        {
            string currentUserId = User.Identity.GetUserId();
            List<IndexTicketViewModel> viewModel;
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                viewModel = DbContext.Tickets
                    .Where(p => p.Project.Archived == false)
                    .Select(
                    ticket => new IndexTicketViewModel
                    {
                        Id = ticket.Id,
                        TicketTitle = ticket.TicketTitle,
                        TicketDescription = ticket.TicketDescription,
                        TicketType = ticket.TicketType.TypeName,
                        TicketPriority = ticket.TicketPriority.PriorityLevel,
                        TicketStatus = ticket.TicketStatus.StatusName,
                        DateCreated = ticket.DateCreated,
                        DateUpdated = ticket.DateUpdated,
                        TicketOwner = new ProjectMemberViewModel()
                        {
                            Id = ticket.TicketOwnerId,
                            NameOfUser = ticket.TicketOwner.NameOfUser,
                            Roles = (from userRoles in ticket.TicketOwner.Roles
                                     join roles in DbContext.Roles
                                     on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        },
                        TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList(),
                        TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList(),
                        ProjectId = ticket.ProjectId,
                        ProjectName = ticket.Project.ProjectName,
                        AvailableForUser = true
                    }).ToList();
            }
            else if (User.IsInRole("Developer") && User.IsInRole("Submitter"))
            {
                // //if only developer
                // DbContext.Tickets.Where(p => p.Project.Users.Any(r => r.Id == currentUserId) ||
                // p.AssignedMembers.Any(r => r.Id == currentUserId));

                // //if only sub
                // DbContext.Tickets.Where(p => p.Project.Users.Any(r => r.Id == currentUserId) ||
                // p.TicketOwnerId == currentUserId);

                // //if both
                // DbContext.Tickets.Where(p => p.Project.Users.Any(r => r.Id == currentUserId) ||
                //p.AssignedMembers.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId);

                viewModel = DbContext.Tickets
                .Where(p => p.Project.Archived == false && ((p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId != currentUserId) || /*2*/ (p.Project.Users.Any(r => r.Id == currentUserId) && !p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*3*/ (p.TicketOwnerId == currentUserId && p.AssignedMembers.Any(r => r.Id == currentUserId) && !p.Project.Users.Any(r => r.Id == currentUserId)) || /*4*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.AssignedMembers.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId)/*5*/))
                .Select(
                    ticket => new IndexTicketViewModel
                    {
                        Id = ticket.Id,
                        TicketTitle = ticket.TicketTitle,
                        TicketDescription = ticket.TicketDescription,
                        TicketType = ticket.TicketType.TypeName,
                        TicketPriority = ticket.TicketPriority.PriorityLevel,
                        TicketStatus = ticket.TicketStatus.StatusName,
                        DateCreated = ticket.DateCreated,
                        DateUpdated = ticket.DateUpdated,
                        TicketOwner = new ProjectMemberViewModel()
                        {
                            Id = ticket.TicketOwnerId,
                            NameOfUser = ticket.TicketOwner.NameOfUser,
                            Roles = (from userRoles in ticket.TicketOwner.Roles
                                     join roles in DbContext.Roles
                                     on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        },
                        TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList(),
                        TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList(),
                        ProjectId = ticket.ProjectId,
                        ProjectName = ticket.Project.ProjectName,
                        AvailableForUser = ticket.TicketOwnerId == currentUserId || ticket.AssignedMembers.Any(w => w.Id == currentUserId)
                    }).ToList();
            }
            else if (User.IsInRole("Developer"))
            {
                viewModel = DbContext.Tickets
                .Where(p => p.Project.Archived == false && (p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId)) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.AssignedMembers.Any(r => r.Id == currentUserId)))
                .Select(
                    ticket => new IndexTicketViewModel
                    {
                        Id = ticket.Id,
                        TicketTitle = ticket.TicketTitle,
                        TicketDescription = ticket.TicketDescription,
                        TicketType = ticket.TicketType.TypeName,
                        TicketPriority = ticket.TicketPriority.PriorityLevel,
                        TicketStatus = ticket.TicketStatus.StatusName,
                        DateCreated = ticket.DateCreated,
                        DateUpdated = ticket.DateUpdated,
                        TicketOwner = new ProjectMemberViewModel()
                        {
                            Id = ticket.TicketOwnerId,
                            NameOfUser = ticket.TicketOwner.NameOfUser,
                            Roles = (from userRoles in ticket.TicketOwner.Roles
                                     join roles in DbContext.Roles
                                     on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        },
                        TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList(),
                        TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList(),
                        ProjectId = ticket.ProjectId,
                        ProjectName = ticket.Project.ProjectName,
                        AvailableForUser = ticket.AssignedMembers.Any(w => w.Id == currentUserId)
                    }).ToList();
            }
            else if (User.IsInRole("Submitter"))
            {
                viewModel = DbContext.Tickets
                .Where(p => p.Project.Archived == false && (p.Project.Users.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId))
                .Select(
                    ticket => new IndexTicketViewModel
                    {
                        Id = ticket.Id,
                        TicketTitle = ticket.TicketTitle,
                        TicketDescription = ticket.TicketDescription,
                        TicketType = ticket.TicketType.TypeName,
                        TicketPriority = ticket.TicketPriority.PriorityLevel,
                        TicketStatus = ticket.TicketStatus.StatusName,
                        DateCreated = ticket.DateCreated,
                        DateUpdated = ticket.DateUpdated,
                        TicketOwner = new ProjectMemberViewModel()
                        {
                            Id = ticket.TicketOwnerId,
                            NameOfUser = ticket.TicketOwner.NameOfUser,
                            Roles = (from userRoles in ticket.TicketOwner.Roles
                                     join roles in DbContext.Roles
                                     on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        },
                        TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList(),
                        TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList(),
                        ProjectId = ticket.ProjectId,
                        ProjectName = ticket.Project.ProjectName,
                        AvailableForUser = ticket.TicketOwnerId == currentUserId
                    }).ToList();
            }
            else
            {
                viewModel = null;
            }

            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult ViewTicket(string id)
        {
            var currentUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);

            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            var model = new IndexTicketViewModel();
            model.Id = ticket.Id;
            model.TicketTitle = ticket.TicketTitle;
            model.TicketDescription = ticket.TicketDescription;
            model.TicketType = ticket.TicketType.TypeName;
            model.TicketPriority = ticket.TicketPriority.PriorityLevel;
            model.TicketStatus = ticket.TicketStatus.StatusName;
            model.DateCreated = ticket.DateCreated;
            model.DateUpdated = ticket.DateUpdated;
            model.TicketOwner = new ProjectMemberViewModel()
            {
                Id = ticket.TicketOwnerId,
                NameOfUser = ticket.TicketOwner.NameOfUser,
                Roles = (from userRoles in ticket.TicketOwner.Roles
                         join roles in DbContext.Roles
                         on userRoles.RoleId equals roles.Id
                         select roles.Name).ToList()
            };
            model.TicketOwner.Id = ticket.TicketOwnerId;
            model.TicketOwner.NameOfUser = ticket.TicketOwner.NameOfUser;
            model.TicketOwner.Roles = (from userRoles in ticket.TicketOwner.Roles
                                       join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                       select roles.Name).ToList();
            model.TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList();
            model.TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList();
            model.ProjectId = ticket.ProjectId;
            model.ProjectName = ticket.Project.ProjectName;
            model.Comments = ticket.Comments
                .Select(comment => new IndexCommentViewModel
                {
                    CommentId = comment.Id,
                    CommentData = comment.CommentData,
                    CommentOwnerId = comment.CommentCreatorId,
                    CommentOwnerName = comment.CommentCreator.NameOfUser,
                    DateCreated = comment.DateCreated,
                    DateUpdated = comment.DateUpdated,
                    AvailableForUser = comment.CommentCreatorId == currentUserId || User.IsInRole("Admin") || User.IsInRole("Project Manager")
                }).ToList();
            model.TicketHistories = ticket.TicketHistories
                .Where(p => p.TicketId == ticket.Id)
                .Select(history => new FullTicketHistory
                {
                    UserName = history.User.NameOfUser,
                    DateUpdated = history.DateUpdated,
                    Changes = history.Changes.Select(r => $"Property {r.PropertyName} changed, from {r.PreviousValue} to {r.ChangedValue} on {r.DateCreated}").ToList()
                }).ToList();

            foreach (var image in ticket.MediaUrls)
            {
                model.MediaUrls.Add(image.MediaUrl);
            }            

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                model.AvailableForUser = true;
            }
            else if (User.IsInRole("Developer") && ticket.AssignedMembers.Any(p => p.Id == currentUserId))
            {
                model.AvailableForUser = true;
            }
            else if (User.IsInRole("Submitter") && ticket.TicketOwnerId == currentUserId)
            {
                model.AvailableForUser = true;
            }
            else
            {
                model.AvailableForUser = false;
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult ManageTicketMembers(string ticketId)
        {
            if (ticketId == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("Index", "Main");
            }

            var model = new ManageTicketMembersViewModel();

            model.TicketName = ticket.TicketTitle;
            model.TicketId = ticket.Id;

            model.CurrentMembers = ticket
                .AssignedMembers.Select(selectuser => new ProjectMemberViewModel
                {
                    Id = selectuser.Id,
                    NameOfUser = selectuser.NameOfUser,
                    Roles = (from userRoles in selectuser.Roles
                             join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                             select roles.Name).ToList()
                }).ToList();

            var idOfDeveloperRole = RolesAndUsersHelper.GetRoleId("Developer");

            model.AllUsers = DbContext.Users
                .Where(p => !p.AssignedTickets.Any(r => r.Id == ticketId) && p.Roles.Any(r => r.RoleId == idOfDeveloperRole))
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
        public async Task<ActionResult> ToggleTicketDeveloper(string ticketId, string userId, string operation)
        {
            if (ticketId == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("Index", "Main");
            }

            if (userId == null || operation == null)
            {
                return RedirectToAction("ManageTicketMembers", "Tickets", new { ticketId = ticketId });
            }

            bool decision;
            List<string> PossibleValues = new List<string> { "Assigned", "Unassigned" };
            string previousValue;
            string currentValue;

            var timeTracker = DbContext.TimeTrackers.FirstOrDefault();

            if (operation == "Add" && !ticket.AssignedMembers.Any(p => p.Id == userId))
            {
                previousValue = PossibleValues[1];
                decision = RolesAndUsersHelper.AssignUserToTicket(ticketId, userId);
                await SendEmail(userId, ticket.TicketTitle, "Add");
                currentValue = PossibleValues[0];
            }
            else if (operation == "Remove" && ticket.AssignedMembers.Any(p => p.Id == userId))
            {
                previousValue = PossibleValues[0];
                decision = RolesAndUsersHelper.UnassignUserFromTicket(ticketId, userId);
                await SendEmail(userId, ticket.TicketTitle, "Remove");
                currentValue = PossibleValues[1];
            }
            else
            {
                previousValue = null;
                decision = false;
                currentValue = null;
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            TicketHistory ticketHistory;
            var propertyName = "AssignedMembers";


            if (decision)
            {
                var ticketHistoryReferenceList = DbContext.TicketHistories.Where(p => p.TicketId == ticket.Id && p.UserId == currentUserId).ToList();


                if (!ticketHistoryReferenceList.Any(p => (DateTime.Now - p.DateUpdated).TotalMinutes < 2))
                {
                    //timeTracker.StartDateTime = DateTime.Now;
                    //timeTracker.UserId = currentUserId;
                    ticketHistory = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        UserId = currentUserId,
                    };
                    ticket.TicketHistories.Add(ticketHistory);
                    currentUser.TicketHistories.Add(ticketHistory);

                    var ticketChange = new TicketChanges
                    {
                        TicketHistoryId = ticketHistory.Id,
                        PropertyName = propertyName,
                        PreviousValue = previousValue,
                        ChangedValue = currentValue
                    };

                    ticketHistory.Changes.Add(ticketChange);
                    ticketHistory.DateUpdated = DateTime.Now;
                }
                else if (ticketHistoryReferenceList.Any(p => (DateTime.Now - p.DateUpdated).TotalMinutes < 2))
                {
                    //timeTracker.EndDateTime = DateTime.Now;
                    ticketHistory = ticketHistoryReferenceList.FirstOrDefault(p => (DateTime.Now - p.DateUpdated).TotalMinutes < 2);

                    var ticketChange = new TicketChanges
                    {
                        TicketHistoryId = ticketHistory.Id,
                        PropertyName = propertyName,
                        PreviousValue = previousValue,
                        ChangedValue = currentValue
                    };

                    ticketHistory.Changes.Add(ticketChange);
                    ticketHistory.DateUpdated = DateTime.Now;
                }
            }

            DbContext.SaveChanges();

            if (decision == true)
            {
                return RedirectToAction("ManageTicketMembers", "Tickets", new { ticketId = ticketId });
            }
            else
            {
                var errorMessage = "Encountered an error!";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction("ManageTicketMembers", "Tickets", new { ticketId = ticketId });
            }
        }

        



        [Authorize]
        private async Task<ActionResult> SendEmail(string userId, string ticketTitle, string operation)
        {
            var user = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            if (user == null)
            {
                RedirectToAction("All Tickets", "Tickets");
            }

            if (operation == "Add")
            {
                await UserManager.SendEmailAsync(userId, $"Assigned to Ticket {ticketTitle}", $"You have been added to ticket {ticketTitle}");
            }
            else if (operation == "Remove")
            {
                await UserManager.SendEmailAsync(userId, $"Unassigned from Ticket {ticketTitle}", $"You have been unassigned from ticket {ticketTitle}");
            }
            else if (operation == "Modify")
            {
                await UserManager.SendEmailAsync(userId, $"Ticket {ticketTitle} has been modified", $"An user just modified ticket {ticketTitle}");
            }
            else
            {
                RedirectToAction("AllTickets", "Tickets");
            }

            return RedirectToAction("AllTickets", "Tickets");
        }





        [HttpGet]
        [Authorize(Roles = "Submitter")]
        public ActionResult _CreateTicket(string projectId)
        {
            var currentUserId = User.Identity.GetUserId();


            if (projectId == null)
            {
                return RedirectToAction("Index", "Main");
            }


            var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId && p.Users.Any(r => r.Id == currentUserId));


            if (project == null || project.Archived == true)
            {
                return RedirectToAction("Index", "Main");
            }


            var model = new CreateEditTicketViewModel();
            model.ProjectId = projectId;

            model.DropDownForPriorities = DbContext.TicketPriorities.Select(p => p.PriorityLevel).ToList();
            model.DropDownForTypes = DbContext.TicketTypes.Select(p => p.TypeName).ToList();
            return /*Json*/View(model/*, JsonRequestBehavior.AllowGet*/);
        }


        [HttpPost]
        [Authorize(Roles = "Submitter")]
        public async Task<ActionResult> _CreateTicket(CreateEditTicketViewModel formdata)
        {
            if (formdata == null)
            {
                return RedirectToAction("Index", "Main");
            }


            if (DbContext.Tickets.Any(p => p.TicketTitle == formdata.TicketTitle && p.ProjectId == formdata.ProjectId))
            {
                ModelState.AddModelError(nameof(CreateEditTicketViewModel.TicketTitle),
                    "Ticket title should be unique");

                return RedirectToAction("_CreateTicket", "Tickets", new { projectId = formdata.ProjectId });
            }

            //formdata.ProjectId = projectId;

            return await SaveTicket(null, formdata);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult _EditTicket(string ticketId)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }


            var isUserAssigned = ticket.AssignedMembers.Any(p => p.Id == currentUserId);
            var isUserOwner = ticket.TicketOwnerId == currentUserId;
            var isUserADeveloperAndSubmitter = User.IsInRole("Developer") && User.IsInRole("Submitter");

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (isUserADeveloperAndSubmitter && !isUserAssigned && !isUserOwner)
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
                else if (User.IsInRole("Developer") && !isUserAssigned && !User.IsInRole("Submitter"))
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
                else if (User.IsInRole("Submitter") && !isUserOwner && !User.IsInRole("Developer"))
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
            }


            var model = new CreateEditTicketViewModel();
            model.TicketId = ticket.Id;
            model.TicketTitle = ticket.TicketTitle;
            model.TicketDescription = ticket.TicketDescription;
            model.TicketType = ticket.TicketType.TypeName;
            model.TicketPriority = ticket.TicketPriority.PriorityLevel;

            foreach (var image in ticket.MediaUrls)
            {
                model.AllUploadedFiles.Add(image.MediaUrl);
            }

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                model.AllProjectNames = currentUser.Projects.Select(p => p.ProjectName).ToList();
                model.TicketStatus = ticket.TicketStatus.StatusName;
                model.DropDownForStatuses = DbContext.TicketStatuses.Select(p => p.StatusName).ToList();
            }
            else
            {
                model.AllProjectNames = currentUser.Projects.Where(p => p.Users.Any(r => r.Id == currentUserId)).Select(t => t.ProjectName).ToList();
            }

            if (model.AllProjectNames.Any() && ticket.Project != null)
            {
                model.ProjectName = ticket.Project.ProjectName;
            }


            model.DropDownForPriorities = DbContext.TicketPriorities.Select(p => p.PriorityLevel).ToList();
            model.DropDownForTypes = DbContext.TicketTypes.Select(p => p.TypeName).ToList();

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public async Task<ActionResult> _EditTicket(CreateEditTicketViewModel formdata)
        {
            if (formdata == null)
            {
                return RedirectToAction("Index", "Main");
            }
            var currentUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == formdata.TicketId);
            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            var isUserAssigned = ticket.AssignedMembers.Any(p => p.Id == currentUserId);
            var isUserOwner = ticket.TicketOwnerId == currentUserId;
            var isUserADeveloperAndSubmitter = User.IsInRole("Developer") && User.IsInRole("Submitter");

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (isUserADeveloperAndSubmitter && !isUserAssigned && !isUserOwner)
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
                else if (User.IsInRole("Developer") && !isUserAssigned && !User.IsInRole("Submitter"))
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
                else if (User.IsInRole("Submitter") && !isUserOwner && !User.IsInRole("Developer"))
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
            }


            if (DbContext.Tickets.Any(p => p.TicketTitle == formdata.TicketTitle && p.ProjectId == formdata.ProjectId && p.Id != formdata.TicketId))
            {
                ModelState.AddModelError(nameof(CreateEditTicketViewModel.TicketTitle),
                    "Ticket title should be unique");

                return RedirectToAction("_CreateTicket", "Tickets", new { projectId = formdata.ProjectId });
            }

            return await SaveTicket(formdata.TicketId, formdata);
        }


        private async Task<ActionResult> SaveTicket(string id, CreateEditTicketViewModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            var ticketType = DbContext.TicketTypes.FirstOrDefault(p => p.TypeName == formdata.TicketType);

            var ticketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.PriorityLevel == formdata.TicketPriority);

            if (ticketType == null || ticketPriority == null)
            {
                return RedirectToAction("_CreateTicket", "Tickets", new { projectId = formdata.ProjectId });
            }

            string fileExtension;

            TicketComparer previousVersionOfTicket;





            Ticket ticket;

            if (id == null)
            {
                ticket = new Ticket();
                ticket.DateCreated = DateTime.Now;
                var ticketStatus = DbContext.TicketStatuses.FirstOrDefault(p => p.StatusName == "Open");

                previousVersionOfTicket = null;

                ticket.TicketTypeId = ticketType.Id;
                ticket.TicketPriorityId = ticketPriority.Id;
                ticket.TicketStatusId = ticketStatus.Id;

                ticketType.Tickets.Add(ticket);
                ticketPriority.Tickets.Add(ticket);
                ticketStatus.Tickets.Add(ticket);

                var project = DbContext.Projects.FirstOrDefault(p => p.Id == formdata.ProjectId);
                if (project == null || project.Archived == true || !project.Users.Any(p => p.Id == userId))
                {
                    return RedirectToAction("Index", "Main");
                }

                ticket.ProjectId = formdata.ProjectId;
                project.Tickets.Add(ticket);

                ticket.TicketOwnerId = currentUser.Id;
                currentUser.OwnedTickets.Add(ticket);

                DbContext.Tickets.Add(ticket);

                //Validating file upload
                if (formdata.Media.Count() > 0 && formdata.Media[0] != null)
                {
                    for (var i = 0; i < formdata.Media.Count(); i++)
                    {
                        fileExtension = Path.GetExtension(formdata.Media[i].FileName);

                        if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("", "File extension is not allowed.");

                            return RedirectToAction("AllTickets", "Tickets");
                        }
                    }
                }


                if (formdata.Media.Count() > 0 && formdata.Media[0] != null)
                {
                    if (!Directory.Exists(Constants.MappedUploadFolder))
                    {
                        Directory.CreateDirectory(Constants.MappedUploadFolder);
                    }

                    for (var i = 0; i < formdata.Media.Count(); i++)
                    {
                        var fileName = formdata.Media[i].FileName;
                        var fullPathWithName = Constants.MappedUploadFolder + fileName;
                        formdata.Media[i].SaveAs(fullPathWithName);
                        var newImage = new FileClass();
                        newImage.AttachmentOwnerId = userId;
                        currentUser.OwnedAttachments.Add(newImage);
                        newImage.MediaUrl = Constants.UploadFolder + fileName;
                        newImage.TicketId = ticket.Id;
                        ticket.MediaUrls.Add(newImage);
                        DbContext.FileClasses.Add(newImage);
                    }
                };
            }
            else
            {

                ticket = DbContext.Tickets.FirstOrDefault(
                    p => p.Id == id);
                previousVersionOfTicket = new TicketComparer
                {
                    Title = ticket.TicketTitle,
                    Description = ticket.TicketDescription,
                    Type = ticket.TicketType.TypeName,
                    Priority = ticket.TicketPriority.PriorityLevel,
                    Status = ticket.TicketStatus.StatusName,
                    ProjectName = ticket.Project.ProjectName
                };


                if (ticket == null || ticket.Project.Archived == true)
                {
                    return RedirectToAction("Index", "Main");
                }


                ticket.DateUpdated = DateTime.Now;

                ticket.TicketTypeId = ticketType.Id;
                ticket.TicketPriorityId = ticketPriority.Id;

                if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
                {
                    var ticketStatus = DbContext.TicketStatuses.FirstOrDefault(p => p.StatusName == formdata.TicketStatus);
                    if (ticketStatus == null)
                    {
                        return RedirectToAction("_CreateTicket", "Tickets", new { projectId = formdata.ProjectId });
                    }

                    ticket.TicketStatusId = ticketStatus.Id;

                }


                if (ticket.Project != null && formdata.ProjectName != ticket.Project.ProjectName)
                {

                    var previousProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);
                    previousProject.Tickets.Remove(ticket);

                    ticket.ProjectId = DbContext.Projects.Where(p => p.Users.Any(r => r.Id == userId)).FirstOrDefault(p => p.ProjectName == formdata.ProjectName).Id;

                    var currentProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);

                    currentProject.Tickets.Add(ticket);
                }
                else if (ticket.Project == null)
                {
                    ticket.ProjectId = DbContext.Projects.Where(p => p.Users.Any(r => r.Id == userId)).FirstOrDefault(p => p.ProjectName == formdata.ProjectName).Id;

                    var currentProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);

                    currentProject.Tickets.Add(ticket);
                }
            }


            //Handling file upload



            ticket.TicketTitle = formdata.TicketTitle;
            ticket.TicketDescription = formdata.TicketDescription;

            DbContext.SaveChanges();



            var currentVersionOfTicket = new TicketComparer
            {
                Title = ticket.TicketTitle,
                Description = ticket.TicketDescription,
                Type = ticket.TicketType.TypeName,
                Priority = ticket.TicketPriority.PriorityLevel,
                Status = ticket.TicketStatus.StatusName,
                ProjectName = ticket.Project.ProjectName
            };

            //DbContext.Entry(ticket).OriginalValues;
            //DbContext.Entry(ticket).CurrentValues;
            //DbContext.Entry(ticket).OriginalValues.PropertyNames;

            //var tracked = DbContext.ChangeTracker.GetType().GetProperties().ToList();

            if (previousVersionOfTicket != null && !currentVersionOfTicket.Equals(previousVersionOfTicket))
            {
                var allProperties = currentVersionOfTicket.GetType().GetProperties().ToList();
                var ticketHistory = new TicketHistory
                {
                    TicketId = ticket.Id,
                    UserId = userId,
                };
                ticket.TicketHistories.Add(ticketHistory);
                currentUser.TicketHistories.Add(ticketHistory);

                foreach (var property in allProperties)
                {
                    ValueRetrieverAndChangeRegistrar(previousVersionOfTicket, currentVersionOfTicket, ticketHistory.Id, property.Name, ticketHistory);
                }

                DbContext.SaveChanges();

                await MassEmailSender(ticket.Id, "Modify");

            }

            return RedirectToAction("ViewTicket", "Tickets", new { id = ticket.Id });
        }



        private async Task<ActionResult> MassEmailSender(string ticketId, string type)
        {
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if(ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
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
                    else if(counter < 2)
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
                    //await SendEmail(user.Id, ticket.TicketTitle, "Modify");
                    //counter += 1;
                }
            }

            return RedirectToAction("AllTickets", "Tickets");
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult EditAttachments(string ticketId)
        {
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);


            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("Index", "Main");
            }


            var model = new EditAttachmentsViewModel();
            model.TicketId = ticketId;
            model.MediaUrls = ticket.MediaUrls.Select(p => p.MediaUrl).ToList();

            if (ticket.TicketOwnerId == currentUserId || ticket.AssignedMembers.Any(p => p.Id == currentUserId) || User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                model.AvailableForUser = true;
            }
            else
            {
                model.AvailableForUser = false;
            }


            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public async Task<ActionResult> AddAttachments(EditAttachmentsViewModel formdata, string ticketId)
        {
            if (formdata == null || ticketId == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (ticket == null || (ticket.TicketOwnerId != currentUserId && !ticket.AssignedMembers.Any(p => p.Id == currentUserId)))
                {
                    return RedirectToAction("AllTickets", "Tickets");
                }
            }


            string fileExtension;

            //Validating file upload
            if (formdata.Files != null)
            {
                for (var i = 0; i < formdata.Files.Count(); i++)
                {
                    fileExtension = Path.GetExtension(formdata.Files[i].FileName);

                    if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", "File extension is not allowed.");

                        return RedirectToAction("AllTickets", "Tickets");
                    }
                }
            }


            //Handling file upload
            if (formdata.Files != null)
            {
                if (!Directory.Exists(Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(Constants.MappedUploadFolder);
                }

                for (var i = 0; i < formdata.Files.Count(); i++)
                {
                    var fileName = formdata.Files[i].FileName;
                    var fullPathWithName = Constants.MappedUploadFolder + fileName;
                    formdata.Files[i].SaveAs(fullPathWithName);
                    var newImage = new FileClass();
                    newImage.AttachmentOwnerId = currentUserId;
                    currentUser.OwnedAttachments.Add(newImage);
                    newImage.MediaUrl = Constants.UploadFolder + fileName;
                    newImage.TicketId = ticket.Id;
                    ticket.MediaUrls.Add(newImage);
                    DbContext.FileClasses.Add(newImage);

                }
            };

            DbContext.SaveChanges();

            await MassEmailSender(ticket.Id, "Modify");

            return RedirectToAction("EditAttachments", "Tickets", new { ticketId = ticketId });
        }


        //[HttpPost]
        //[Authorize(Roles = "Admin, Project Manager, Submitter")]
        //public ActionResult ArchiveTickets(string ticketId)
        //{
        //    if(ticketId == null)
        //    {
        //        return RedirectToAction("AllTickets", "Tickets");
        //    }
        //    var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
        //    var currentUserId = User.Identity.GetUserId();
        //    var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
        //    var project = DbContext.Projects.FirstOrDefault(p => p.Tickets.Contains(ticket));

        //    if (ticket == null || project == null)
        //    {
        //        return RedirectToAction("AllTickets", "Tickets");
        //    }

        //    ticket.MediaUrls.Clear();
        //    DbContext.Tickets.Remove(ticket);
        //    project.Tickets.Remove(ticket);


        //}



        


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public async Task<ActionResult> DeleteAttachments(string filename, string ticketId)
        {
            if (filename == null || ticketId == null)
            {
                return RedirectToAction("Index", "Main");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if(ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (ticket == null || (ticket.TicketOwnerId != currentUserId && !ticket.AssignedMembers.Any(p => p.Id == currentUserId)))
                {
                    return RedirectToAction("Index", "Main");
                }
            }

            var attachment = ticket.MediaUrls.FirstOrDefault(p => p.MediaUrl == filename);
            if (attachment == null)
            {
                return RedirectToAction("EditAttachments", "Tickets", new { ticketId = ticketId });
            }

            if(attachment.AttachmentOwnerId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                return RedirectToAction("EditAttachments", "Tickets", new { ticketId = ticket.Id });
            }


            ticket.MediaUrls.Remove(attachment);
            currentUser.OwnedAttachments.Remove(attachment);
            DbContext.FileClasses.Remove(attachment);

            DbContext.SaveChanges();

            await MassEmailSender(ticket.Id, "Modify");

            return RedirectToAction("EditAttachments", "Tickets", new { ticketId = ticketId });
        }



        private void ValueRetrieverAndChangeRegistrar(TicketComparer comparingObject, TicketComparer objectBeingComparedTo, string ticketHistoryId, string propertyName, TicketHistory ticketHistory)
        {
            bool decision;
            var valueBeingCompared = comparingObject.GetType().GetProperty(propertyName).GetValue(comparingObject);
            var correspondingValueOfCurrentObject = objectBeingComparedTo.GetType().GetProperty(propertyName).GetValue(objectBeingComparedTo);

            if (valueBeingCompared is string && correspondingValueOfCurrentObject is string && !string.Equals(valueBeingCompared, correspondingValueOfCurrentObject))
            {
                decision = true;
            }
            else
            {
                decision = false;
            }

            if (decision)
            {
                var ticketChange = new TicketChanges
                {
                    PropertyName = propertyName,
                    PreviousValue = valueBeingCompared.ToString(),
                    ChangedValue = correspondingValueOfCurrentObject.ToString(),
                    TicketHistoryId = ticketHistoryId,
                };

                ticketHistory.Changes.Add(ticketChange);
                ticketHistory.DateUpdated = DateTime.Now;
            }
        }


        private double Checker(DateTime startTime, DateTime endTime)
        {
            var timeSpan = startTime.Subtract(endTime);

            return timeSpan.TotalMinutes;
        }



        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult TicketNotificationIndex(/*string ticketId, string operation*/)
        {
            var currentUserId = User.Identity.GetUserId();
            //var subscriptionTickets = DbContext.AdminAndProjectManagerClasses.FirstOrDefault(p => p.UserId == currentUserId);

            //var subscribedTickets = subscriptionTickets.SubscribedTickets;
            //var unsubscribedTickets = DbContext.Tickets.Where(p => !subscribedTickets.Any(r => r.Id == p.Id)).ToList();
            var model = new ToggleNotificationViewModel();
            model.AllUnsubscribedTickets = DbContext.Tickets.Where(p => p.Project.Archived == false && !p.SubscribedUsers.Any(r => r.Id == currentUserId)).Select(
                    ticket => new IndexTicketViewModel
                    {
                        Id = ticket.Id,
                        TicketTitle = ticket.TicketTitle,
                        TicketDescription = ticket.TicketDescription,
                        TicketType = ticket.TicketType.TypeName,
                        TicketPriority = ticket.TicketPriority.PriorityLevel,
                        TicketStatus = ticket.TicketStatus.StatusName,
                        DateCreated = ticket.DateCreated,
                        DateUpdated = ticket.DateUpdated,
                        TicketOwner = new ProjectMemberViewModel()
                        {
                            Id = ticket.TicketOwnerId,
                            NameOfUser = ticket.TicketOwner.NameOfUser,
                            Roles = (from userRoles in ticket.TicketOwner.Roles
                                     join roles in DbContext.Roles
                                     on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        },
                        TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList(),
                        TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList(),
                        ProjectId = ticket.ProjectId,
                        ProjectName = ticket.Project.ProjectName
                    }).ToList();

            model.AllSubsribedTickets = DbContext.Tickets.Where(p => p.Project.Archived == false && p.SubscribedUsers.Any(r => r.Id == currentUserId)).Select(
                    ticket => new IndexTicketViewModel
                    {
                        Id = ticket.Id,
                        TicketTitle = ticket.TicketTitle,
                        TicketDescription = ticket.TicketDescription,
                        TicketType = ticket.TicketType.TypeName,
                        TicketPriority = ticket.TicketPriority.PriorityLevel,
                        TicketStatus = ticket.TicketStatus.StatusName,
                        DateCreated = ticket.DateCreated,
                        DateUpdated = ticket.DateUpdated,
                        TicketOwner = new ProjectMemberViewModel()
                        {
                            Id = ticket.TicketOwnerId,
                            NameOfUser = ticket.TicketOwner.NameOfUser,
                            Roles = (from userRoles in ticket.TicketOwner.Roles
                                     join roles in DbContext.Roles
                                     on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        },
                        TicketAssignees = ticket.AssignedMembers
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        }).ToList(),
                        TicketNonAssignees = DbContext.Users
                        .Where(p => !p.AssignedTickets.Any(r => r.Id == ticket.Id))
                        .Select(selectuser => new ProjectMemberViewModel
                        {
                            Id = selectuser.Id,
                            NameOfUser = selectuser.NameOfUser,
                            Roles = (from userRoles in selectuser.Roles
                                     join roles in DbContext.Roles on userRoles.RoleId equals roles.Id
                                     select roles.Name).ToList()
                        })
                        .ToList(),
                        ProjectId = ticket.ProjectId,
                        ProjectName = ticket.Project.ProjectName
                    }).ToList();

            return View(model);
        }



        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult TicketToSubscription(string ticketId, string operation)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (currentUser == null || ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Main");
            }

            //var subscriptionTickets = DbContext.AdminAndProjectManagerClasses.FirstOrDefault(p => p.UserId == currentUserId);

            if (operation == "Add" && !ticket.SubscribedUsers.Any(p => p.Id == currentUserId))
            {
                ticket.SubscribedUsers.Add(currentUser);
                currentUser.AdminAndProjectManagerNotificationTickets.Add(ticket);
            }
            else if (operation == "Remove" && ticket.SubscribedUsers.Any(p => p.Id == currentUserId))
            {
                ticket.SubscribedUsers.Remove(currentUser);
                currentUser.AdminAndProjectManagerNotificationTickets.Remove(ticket);
            }

            DbContext.SaveChanges();

            return RedirectToAction("TicketNotificationIndex", "Tickets");
        }

    }
}