using BugTracker.Models;
using BugTracker.Models.Domain;
using BugTracker.Models.Helpers;
using BugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult CreateComment(IndexTicketViewModel formdata, string ticketId)
        {
            var currentUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);
            if(ticket == null)
            {
                return RedirectToAction(nameof(TicketsController.AllTickets));
            }
            var currentUser = DbContext.Users.FirstOrDefault(p => p.Id == currentUserId);


            var isUserAssigned = ticket.AssignedMembers.Any(p => p.Id == currentUserId);
            var isUserOwner = ticket.TicketOwnerId == currentUserId;
            var isUserADeveloperAndSubmitter = User.IsInRole("Developer") && User.IsInRole("Submitter");

            if (!User.IsInRole("Admin") && !User.IsInRole("Project Manager"))
            {

                if (isUserADeveloperAndSubmitter && !isUserAssigned && !isUserOwner)
                {
                    return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
                }
                else if (User.IsInRole("Developer") && !isUserAssigned && !User.IsInRole("Submitter"))
                {
                    return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
                }
                else if (User.IsInRole("Submitter") && !isUserOwner && !User.IsInRole("Developer"))
                {
                    return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
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

            return RedirectToAction(nameof(TicketsController.ViewTicket), new { id = ticketId });
        }        
    }
}