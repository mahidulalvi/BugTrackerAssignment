using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTracker.Models.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty("Users")]
        public virtual List<Project> Projects { get; set; }

        public string NameOfUser { get; set; }

        [InverseProperty("User")]
        public virtual List<Project> CreatedProjects { get; set; }
        

        [InverseProperty("AssignedMembers")]
        public virtual List<Ticket> AssignedTickets { get; set; }

        [InverseProperty("TicketOwner")]
        public virtual List<Ticket> OwnedTickets { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public virtual List<TicketHistory> TicketHistories { get; set; }

        [InverseProperty("SubscribedUsers")]
        public virtual List<Ticket> AdminAndProjectManagerNotificationTickets { get; set; }

        public virtual List<FileClass> OwnedAttachments { get; set; }
        
        public ApplicationUser()
        {
            Projects = new List<Project>();
            CreatedProjects = new List<Project>();
            AssignedTickets = new List<Ticket>();
            OwnedTickets = new List<Ticket>();
            Comments = new List<Comment>();
            TicketHistories = new List<TicketHistory>();
            AdminAndProjectManagerNotificationTickets = new List<Ticket>();
            OwnedAttachments = new List<FileClass>();
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DBConnectionString", throwIfV1Schema: false)
        {
        }

        public DbSet<Project> Projects { set; get; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<FileClass> FileClasses { get; set; }

        public DbSet<TicketPriority> TicketPriorities { get; set; }

        public DbSet<TicketStatus> TicketStatuses { get; set; }

        public DbSet<TicketType> TicketTypes { get; set; }

        public DbSet<TicketHistory> TicketHistories { get; set; }

        public DbSet<TicketChanges> TicketChanges { get; set; }

        public DbSet<TimeTracker> TimeTrackers { get; set; }

        public DbSet<ActionLog> ActionLogs { get; set; }

        public DbSet<ExceptionLog> ExceptionLogs { get; set; }    
                

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}