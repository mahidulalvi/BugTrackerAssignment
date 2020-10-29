using System;

namespace BugTracker.Models.Domain
{
    public class TimeTracker
    {
        public string Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public string UserId { get; set; }
        public string Identifyer { get; set; }
        public int TimesDone { get; set; }

        public TimeTracker()
        {
            Id = Guid.NewGuid().ToString();
            StartDateTime = DateTime.Now;
            EndDateTime = DateTime.Now;
            TimesDone = 0;
        }

        public double Checker(DateTime comparingTime)
        {            
            TimeSpan = comparingTime.Subtract(EndDateTime);

            return TimeSpan.TotalMilliseconds;
        }

        public bool IsInRange(int specifiedMiliSeconds, DateTime comparingTime)
        {
            if (Checker(comparingTime) < specifiedMiliSeconds)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}