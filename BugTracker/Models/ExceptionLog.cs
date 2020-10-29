using System;

namespace BugTracker.Models
{
    public class ExceptionLog
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }

        public ExceptionLog()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now;
        }
    }
}