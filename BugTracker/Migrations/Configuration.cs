namespace BugTracker.Migrations
{
    using BugTracker.Models;
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
                new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

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
            //ApplicationUser moderatorUser;
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


            //if (!context.Users.Any(
            //    p => p.UserName == "moderator@blog.com"))
            //{
            //    moderatorUser = new ApplicationUser();
            //    moderatorUser.UserName = "moderator@blog.com";
            //    moderatorUser.Email = "moderator@blog.com";
            //    userManager.Create(moderatorUser, "Password-1");
            //}
            //else
            //{
            //    moderatorUser = context
            //        .Users
            //        .First(p => p.UserName == "moderator@blog.com");
            //}

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            //if (!userManager.IsInRole(moderatorUser.Id, "Moderator"))
            //{
            //    userManager.AddToRole(moderatorUser.Id, "Moderator");
            //}


        }
    }
}
