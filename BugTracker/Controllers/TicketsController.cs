using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext DbContext;
        private RolesAndUsersHelper RolesAndUsersHelper;


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
                        ProjectName = ticket.Project.ProjectName
                    }).ToList();
            }
            else if (User.IsInRole("Developer") && User.IsInRole("Submitter"))
            {
                viewModel = DbContext.Tickets
                .Where(p => ((p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId != currentUserId) || /*2*/ (p.Project.Users.Any(r => r.Id == currentUserId) && !p.AssignedMembers.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*3*/ (p.TicketOwnerId == currentUserId && p.AssignedMembers.Any(r => r.Id == currentUserId) && !p.Project.Users.Any(r => r.Id == currentUserId)) || /*4*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.AssignedMembers.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId)/*5*/))
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
                        ProjectName = ticket.Project.ProjectName
                    }).ToList();
            }
            else if (User.IsInRole("Developer"))
            {
                viewModel = DbContext.Tickets
                .Where(p => (p.Project.Users.Any(r => r.Id == currentUserId) && p.AssignedMembers.Any(r => r.Id == currentUserId)) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.AssignedMembers.Any(r => r.Id == currentUserId)))
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
                        ProjectName = ticket.Project.ProjectName
                    }).ToList();
            }
            else if (User.IsInRole("Submitter"))
            {
                viewModel = DbContext.Tickets
                .Where(p => (p.Project.Users.Any(r => r.Id == currentUserId) && p.TicketOwnerId == currentUserId) || /*1*/ (p.Project.Users.Any(r => r.Id == currentUserId) || p.TicketOwnerId == currentUserId))
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
                        ProjectName = ticket.Project.ProjectName
                    }).ToList();
            }
            else
            {
                viewModel = null;
            }

            //var a = DbContext.Tickets.First();
            //var b = DbContext.Projects.ToList();

            return View(viewModel);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult ViewTicket(string id)
        {
            var currentUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);

            if(ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.AllTickets));
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

            foreach (var image in ticket.MediaUrls)
            {
                model.MediaUrls.Add(image.MediaUrl);
            }

            foreach (var comment in ticket.Comments)
            {
                model.Comments.Add(comment.CommentData);
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
                return RedirectToAction(nameof(MainController.Index));
            }

            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (ticket == null)
            {
                return RedirectToAction(nameof(MainController.Index));
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
        public ActionResult ToggleTicketDeveloper(string ticketId, string userId, string operation)
        {
            if (ticketId == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            if (userId == null || operation == null)
            {
                return RedirectToAction(nameof(TicketsController.ManageTicketMembers), new { ticketId = ticketId });
            }

            bool decision;

            if (operation == "Add" && !ticket.AssignedMembers.Any(p => p.Id == userId))
            {
                decision = RolesAndUsersHelper.AssignUserToTicket(ticketId, userId);
            }
            else if (operation == "Remove" && ticket.AssignedMembers.Any(p => p.Id == userId))
            {
                decision = RolesAndUsersHelper.UnassignUserFromTicket(ticketId, userId);
            }
            else
            {
                decision = false;
            }

            DbContext.SaveChanges();

            if (decision == true)
            {
                return RedirectToAction(nameof(TicketsController.ManageTicketMembers), new { ticketId = ticketId });
            }
            else
            {
                var errorMessage = "Encountered an error!";
                TempData["errorMessage"] = errorMessage;
                return RedirectToAction(nameof(TicketsController.ManageTicketMembers), new { ticketId = ticketId });
            }
        }


        [HttpGet]
        [Authorize(Roles = "Submitter")]
        public ActionResult _CreateTicket(string projectId)
        {
            var currentUserId = User.Identity.GetUserId();


            if (projectId == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }


            var project = DbContext.Projects.FirstOrDefault(p => p.Id == projectId && p.Users.Any(r => r.Id == currentUserId));


            if (project == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }


            var model = new CreateEditTicketViewModel();
            model.ProjectId = projectId;

            model.DropDownForPriorities = DbContext.TicketPriorities.Select(p => p.PriorityLevel).ToList();
            model.DropDownForTypes = DbContext.TicketTypes.Select(p => p.TypeName).ToList();
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Submitter")]
        public ActionResult _CreateTicket(CreateEditTicketViewModel formdata)
        {
            if (formdata == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }


            if (DbContext.Tickets.Any(p => p.TicketTitle == formdata.TicketTitle && p.ProjectId == formdata.ProjectId))
            {
                ModelState.AddModelError(nameof(CreateEditTicketViewModel.TicketTitle),
                    "Ticket title should be unique");            

                return RedirectToAction(nameof(TicketsController._CreateTicket), new { projectId = formdata.ProjectId })/*View()*/;
            }

            return SaveTicket(null, formdata);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult _EditTicket(string ticketId)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if(ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.AllTickets));
            }


            var isUserAssigned = ticket.AssignedMembers.Any(p => p.Id == currentUserId);
            var isUserOwner = ticket.TicketOwnerId == currentUserId;
            var isUserADeveloperAndSubmitter = User.IsInRole("Developer") && User.IsInRole("Submitter");


            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (isUserADeveloperAndSubmitter && !isUserAssigned && !isUserOwner)
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
                }
                else if (User.IsInRole("Developer") && !isUserAssigned && !User.IsInRole("Submitter"))
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
                }
                else if (User.IsInRole("Submitter") && !isUserOwner && !User.IsInRole("Developer"))
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
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

            if (model.AllProjectNames.Any())
            {
                model.ProjectName = ticket.Project.ProjectName;
            }


            model.DropDownForPriorities = DbContext.TicketPriorities.Select(p => p.PriorityLevel).ToList();
            model.DropDownForTypes = DbContext.TicketTypes.Select(p => p.TypeName).ToList();

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult _EditTicket(CreateEditTicketViewModel formdata)
        {
            if (formdata == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }
            var currentUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == formdata.TicketId);
            if(ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.AllTickets));
            }

            var isUserAssigned = ticket.AssignedMembers.Any(p => p.Id == currentUserId);
            var isUserOwner = ticket.TicketOwnerId == currentUserId;
            var isUserADeveloperAndSubmitter = User.IsInRole("Developer") && User.IsInRole("Submitter");

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (isUserADeveloperAndSubmitter && !isUserAssigned && !isUserOwner)
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
                }
                else if (User.IsInRole("Developer") && !isUserAssigned && !User.IsInRole("Submitter"))
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
                }
                else if (User.IsInRole("Submitter") && !isUserOwner && !User.IsInRole("Developer"))
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
                }
            }


            if (DbContext.Tickets.Any(p => p.TicketTitle == formdata.TicketTitle && p.ProjectId == formdata.ProjectId))
            {
                ModelState.AddModelError(nameof(CreateEditTicketViewModel.TicketTitle),
                    "Ticket title should be unique");

                return RedirectToAction(nameof(TicketsController._CreateTicket), new { projectId = formdata.ProjectId })/*View()*/;
            }

            return SaveTicket(formdata.TicketId, formdata);
        }


        private ActionResult SaveTicket(string id, CreateEditTicketViewModel formdata)
        {
            var userId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == userId);

            var ticketType = DbContext.TicketTypes.FirstOrDefault(p => p.TypeName == formdata.TicketType);

            var ticketPriority = DbContext.TicketPriorities.FirstOrDefault(p => p.PriorityLevel == formdata.TicketPriority);

            if (ticketType == null || ticketPriority == null)
            {
                return RedirectToAction(nameof(TicketsController._CreateTicket), new { projectId = formdata.ProjectId });
            }

            string fileExtension;

            //Validating file upload
            if (formdata.Media.Count() > 0 && formdata.Media[0] != null)
            {
                for (var i = 0; i < formdata.Media.Count(); i++)
                {
                    fileExtension = Path.GetExtension(formdata.Media[i].FileName);

                    if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", "File extension is not allowed.");

                        return RedirectToAction(nameof(TicketsController.AllTickets));
                    }
                }
            }


            Ticket ticket;

            if (id == null)
            {
                ticket = new Ticket();
                ticket.DateCreated = DateTime.Now;
                var ticketStatus = DbContext.TicketStatuses.FirstOrDefault(p => p.StatusName == "Open");

                ticket.TicketTypeId = ticketType.Id;
                ticket.TicketPriorityId = ticketPriority.Id;
                ticket.TicketStatusId = ticketStatus.Id;

                ticketType.Tickets.Add(ticket);
                ticketPriority.Tickets.Add(ticket);
                ticketStatus.Tickets.Add(ticket);

                var project = DbContext.Projects.FirstOrDefault(p => p.Id == formdata.ProjectId);
                if (project == null || !project.Users.Any(p => p.Id == userId))
                {
                    return RedirectToAction(nameof(MainController.Index), nameof(MainController));
                }

                ticket.ProjectId = formdata.ProjectId;
                project.Tickets.Add(ticket);

                ticket.TicketOwnerId = currentUser.Id;
                currentUser.OwnedTickets.Add(ticket);

                DbContext.Tickets.Add(ticket);
            }
            else
            {
                ticket = DbContext.Tickets.FirstOrDefault(
                    p => p.Id == id);

                if (ticket == null)
                {
                    return RedirectToAction(nameof(MainController.Index));
                }


                ticket.DateUpdated = DateTime.Now;

                ticket.TicketTypeId = ticketType.Id;
                ticket.TicketPriorityId = ticketPriority.Id;

                if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
                {
                    var ticketStatus = DbContext.TicketStatuses.FirstOrDefault(p => p.StatusName == formdata.TicketStatus);
                    if (ticketStatus == null)
                    {
                        return RedirectToAction(nameof(TicketsController._CreateTicket), new { projectId = formdata.ProjectId });
                    }

                    ticket.TicketStatusId = ticketStatus.Id;

                }


                if (formdata.ProjectName != ticket.Project.ProjectName)
                {
                    var previousProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);
                    previousProject.Tickets.Remove(ticket);

                    ticket.ProjectId = DbContext.Projects.Where(p => p.Users.Any(r => r.Id == userId)).FirstOrDefault(p => p.ProjectName == formdata.ProjectName).Id;

                    var currentProject = DbContext.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);

                    currentProject.Tickets.Add(ticket);
                }
            }


            //Handling file upload
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
                    newImage.MediaUrl = Constants.UploadFolder + fileName;
                    newImage.TicketId = ticket.Id;
                    ticket.MediaUrls.Add(newImage);
                    DbContext.FileClasses.Add(newImage);
                }
            };


            ticket.TicketTitle = formdata.TicketTitle;
            ticket.TicketDescription = formdata.TicketDescription;

            DbContext.SaveChanges();

            return RedirectToAction("ViewTicket", "Tickets", new { id = ticket.Id });
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult EditAttachments(string ticketId)
        {
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var model = new EditAttachmentsViewModel();
            model.TicketId = ticketId;
            model.MediaUrls = ticket.MediaUrls.Select(p => p.MediaUrl).ToList();


            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult AddAttachments(EditAttachmentsViewModel formdata, string ticketId)
        {
            if (formdata == null || ticketId == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (ticket == null || (ticket.TicketOwnerId != currentUserId && !ticket.AssignedMembers.Any(p => p.Id == currentUserId)))
                {
                    return RedirectToAction(nameof(TicketsController.AllTickets));
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

                        return RedirectToAction(nameof(TicketsController.AllTickets));
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
                    newImage.MediaUrl = Constants.UploadFolder + fileName;
                    newImage.TicketId = ticket.Id;
                    ticket.MediaUrls.Add(newImage);
                    DbContext.FileClasses.Add(newImage);
                }
            };

            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketsController.EditAttachments), new { ticketId = ticketId });
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public ActionResult DeleteAttachments(string filename, string ticketId)
        {
            if (filename == null || ticketId == null)
            {
                return RedirectToAction(nameof(MainController.Index));
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {
                if (ticket == null || (ticket.TicketOwnerId != currentUserId && !ticket.AssignedMembers.Any(p => p.Id == currentUserId)))
                {
                    return RedirectToAction(nameof(MainController.Index));
                }
            }

            var attachment = ticket.MediaUrls.FirstOrDefault(p => p.MediaUrl == filename);
            if (attachment == null)
            {
                return RedirectToAction(nameof(TicketsController.EditAttachments), new { ticketId = ticketId });
            }


            ticket.MediaUrls.Remove(attachment);
            DbContext.FileClasses.Remove(attachment);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(TicketsController.EditAttachments), new { ticketId = ticketId });
        }

    }
}