using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class CommentsController : Controller
    {
        // GET: Comments
        private ApplicationDbContext DbContext;
        private RolesAndUsersHelper RolesAndUsersHelper;        


        public CommentsController()
        {
            DbContext = new ApplicationDbContext();
            RolesAndUsersHelper = new RolesAndUsersHelper(DbContext);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        public async Task<ActionResult> CreateComment(IndexTicketViewModel formdata, string ticketId)
        {
            var currentUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if (ticket == null || ticket.Project.Archived == true)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);


            var isUserAssigned = ticket.AssignedMembers.Any(p => p.Id == currentUserId);
            var isUserOwner = ticket.TicketOwnerId == currentUserId;
            var isUserADeveloperAndSubmitter = User.IsInRole("Developer") && User.IsInRole("Submitter");

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {

                if (isUserADeveloperAndSubmitter && !isUserAssigned && !isUserOwner)
                {
                    return RedirectToAction("ViewTicket", "Tickets", new { id = ticketId });
                }
                else if (User.IsInRole("Developer") && !isUserAssigned && !User.IsInRole("Submitter"))
                {
                    return RedirectToAction("ViewTicket", "Tickets", new { id = ticketId });
                }
                else if (User.IsInRole("Submitter") && !isUserOwner && !User.IsInRole("Developer"))
                {
                    return RedirectToAction("ViewTicket", "Tickets", new { id = ticketId });
                }
            }


            var comment = new Comment();
            comment.CommentCreatorId = currentUserId;
            comment.TicketId = ticketId;
            comment.CommentData = formdata.CommentData;

            ticket.Comments.Add(comment);
            currentUser.Comments.Add(comment);

            DbContext.Comments.Add(comment);

            DbContext.SaveChanges();

            await RolesAndUsersHelper.MassEmailSender(ticket.Id, "Modify");

            return RedirectToAction("ViewTicket", "Tickets", new { id = ticketId });
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Submitter")]
        public ActionResult _EditComment(string commentId)
        {
            if (commentId == null)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == commentId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Comments.Any(r => r.Id == comment.Id));

            if (currentUser == null || comment == null || ticket == null || ticket.Project.Archived == true || (comment.CommentCreatorId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Project Manager")))
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            var model = new CreateEditCommentViewModel();
            model.CommentData = comment.CommentData;
            model.CommentId = comment.Id;
            model.TicketId = ticket.Id;

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Submitter")]
        public async Task<ActionResult> _EditComment(CreateEditCommentViewModel formData)
        {
            if (formData == null)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == formData.CommentId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == formData.TicketId);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);

            if (comment == null || ticket == null || ticket.Project.Archived == true || formData.CommentData == null || (comment.CommentCreatorId != currentUserId && !User.IsInRole("Admin") && !User.IsInRole("Project Manager")))
            {
                return RedirectToAction("AllTickets", "Tickets");
            }

            comment.CommentData = formData.CommentData;
            comment.DateUpdated = DateTime.Now;

            DbContext.SaveChanges();

            await RolesAndUsersHelper.MassEmailSender(ticket.Id, "Modify");

            return RedirectToAction("ViewTicket", "Tickets", new { id = ticket.Id });
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Submitter")]
        public ActionResult DeleteComment(string commentId)
        {
            if (commentId == null)
            {
                return RedirectToAction("AllTickets", "Tickets");
            }
            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == commentId);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Comments.Any(r => r.Id == comment.Id));

            if (ticket == null || ticket.Project.Archived == true || comment == null)
            {
                return RedirectToAction("ViewTicket", "Tickets", new { ticketId = ticket.Id });
            }

            ticket.Comments.Remove(comment);
            if (comment.CommentCreatorId == currentUserId)
            {
                currentUser.Comments.Remove(comment);
            }
            DbContext.Comments.Remove(comment);

            DbContext.SaveChanges();

            return RedirectToAction("ViewTicket", "Tickets", new { id = ticket.Id });
        }
       
    }
}