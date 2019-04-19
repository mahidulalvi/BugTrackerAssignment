namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using BugTracker.Models.Domain;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.


            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));


            var userManager =
                new ApplicationUserManager(
                    new UserStore<ApplicationUser>(context));


            //If the app needs to create users with -on the name, we need to set the validator.
           userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
           {
               AllowOnlyAlphanumericUserNames = false,
               RequireUniqueEmail = true               
           };



            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            if (!context.Roles.Any(p => p.Name == "Project Manager"))
            {
                var projectManagerRole = new IdentityRole("Project Manager");
                roleManager.Create(projectManagerRole);
            }

            if (!context.Roles.Any(p => p.Name == "Developer"))
            {
                var developerRole = new IdentityRole("Developer");
                roleManager.Create(developerRole);
            }

            if (!context.Roles.Any(p => p.Name == "Submitter"))
            {
                var submitterRole = new IdentityRole("Submitter");
                roleManager.Create(submitterRole);
            }

            //Creating the adminuser
            ApplicationUser adminUser;


            if (!context.Users.Any(
                p => p.UserName == "admin@mybugtracker.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@mybugtracker.com";
                adminUser.Email = "admin@mybugtracker.com";
                adminUser.EmailConfirmed = true;
                adminUser.NameOfUser = "admin";
                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@mybugtracker.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }


            //Seeding ticket priorities
            TicketPriority ticketPriorityLow = new TicketPriority();
            TicketPriority ticketPriorityMedium = new TicketPriority();
            TicketPriority ticketPriorityHigh = new TicketPriority();

            
            ticketPriorityLow.PriorityLevel = "Low";
            ticketPriorityMedium.PriorityLevel = "Medium";
            ticketPriorityHigh.PriorityLevel = "High";
            

            context.TicketPriorities.AddOrUpdate(p => p.PriorityLevel, ticketPriorityLow);
            context.TicketPriorities.AddOrUpdate(p => p.PriorityLevel, ticketPriorityMedium);
            context.TicketPriorities.AddOrUpdate(p => p.PriorityLevel, ticketPriorityHigh);
            //Seeding of ticket priorities ends here

            //Seeding of ticket statuses starts here
            TicketStatus ticketStatusOpen = new TicketStatus();
            TicketStatus ticketStatusResolved = new TicketStatus();
            TicketStatus ticketStatusRejected = new TicketStatus();

            ticketStatusOpen.StatusName = "Open";
            ticketStatusResolved.StatusName = "Resolved";
            ticketStatusRejected.StatusName = "Rejected";

            context.TicketStatuses.AddOrUpdate(p => p.StatusName, ticketStatusOpen);
            context.TicketStatuses.AddOrUpdate(p => p.StatusName, ticketStatusResolved);
            context.TicketStatuses.AddOrUpdate(p => p.StatusName, ticketStatusRejected);
            //Seeding of ticket statuses ends here

            //Seeding of ticket types starts here
            TicketType ticketTypeBug = new TicketType();
            TicketType ticketTypeFeature = new TicketType();
            TicketType ticketTypeDatabase = new TicketType();
            TicketType ticketTypeSupport = new TicketType();

            ticketTypeBug.TypeName = "Bug";
            ticketTypeFeature.TypeName = "Feature";
            ticketTypeDatabase.TypeName = "Database";
            ticketTypeSupport.TypeName = "Support";

            context.TicketTypes.AddOrUpdate(p => p.TypeName, ticketTypeBug);
            context.TicketTypes.AddOrUpdate(p => p.TypeName, ticketTypeFeature);
            context.TicketTypes.AddOrUpdate(p => p.TypeName, ticketTypeDatabase);
            context.TicketTypes.AddOrUpdate(p => p.TypeName, ticketTypeSupport);
            //Seeding of ticket types ends here

            context.SaveChanges();
        }
    }
}
