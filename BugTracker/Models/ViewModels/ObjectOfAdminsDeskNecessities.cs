using System.Collections.Generic;

namespace BugTracker.Models.ViewModels
{
    public class ObjectOfAdminsDeskNecessities
    {
        public string ProvidedUserId { get; set; }
        public List<string> ProvidedRoles { get; set; }
        public string ProvidedUserName { get; set; }
        public List<string> AllRoles { get; set; }

        public ObjectOfAdminsDeskNecessities()
        {
            AllRoles = new List<string>();
        }
    }
}