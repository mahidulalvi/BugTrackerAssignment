using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class ObjectOfAdminsDeskNecessities
    {
        public string ProvidedUserId { get; set; }
        public ICollection<string> ProvidedRoles { get; set; }
        public string ProvidedUserName { get; set; }
        public List<string> AllRoles { get; set; }

        public ObjectOfAdminsDeskNecessities()
        {
            AllRoles = new List<string>();
        }
    }
}