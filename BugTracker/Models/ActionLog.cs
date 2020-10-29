using System;

namespace BugTracker.Models
{
    public class ActionLog
    {
        public string Id { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public DateTime DateCreated { get; set; }

        public ActionLog()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
        }
    }
}
