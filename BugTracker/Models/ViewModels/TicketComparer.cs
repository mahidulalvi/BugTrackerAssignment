using BugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.ViewModels
{
    public class TicketComparer
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string ProjectName { get; set; }
        public List<string> AssignedMembers { get; set; }
        public List<string> MediaUrls { get; set; }
        public List<string> Comments { get; set; }

        public TicketComparer()
        {
            AssignedMembers = new List<string>();
            MediaUrls = new List<string>();
            Comments = new List<string>();
        }


        public override bool Equals(Object obj)
        {
            TicketComparer comparerTicket = obj as TicketComparer;
            if (comparerTicket == null)
            {
                return false;
            }
            else
            {
                if (string.Equals(Title, comparerTicket.Title) && string.Equals(Description, comparerTicket.Description) && string.Equals(Type, comparerTicket.Type) && string.Equals(Status, comparerTicket.Status) && string.Equals(Priority, comparerTicket.Priority) && string.Equals(ProjectName, comparerTicket.ProjectName) && IsComparingTicketEqual(comparerTicket))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                    ;
            }
        }


        public override int GetHashCode()
        {
            return (this.AssignedMembers.Count() + this.Comments.Count() + this.MediaUrls.Count()).GetHashCode();
        }


        private bool IsComparingTicketEqual(TicketComparer comparerTicket)
        {
            var boolOfCounts = ((AssignedMembers.Count() == comparerTicket.AssignedMembers.Count()) && (MediaUrls.Count() == comparerTicket.MediaUrls.Count()) && (Comments.Count() == comparerTicket.MediaUrls.Count()));

            var falseDetected = false;

            if (this.GetHashCode() == comparerTicket.GetHashCode())
            {
                foreach (var element in comparerTicket.AssignedMembers)
                {
                    if (!AssignedMembers.Contains(element))
                    {
                        falseDetected = true;
                    }
                }

                foreach (var element in comparerTicket.Comments)
                {
                    if (!Comments.Contains(element))
                    {
                        falseDetected = true;
                    }
                }

                foreach (var element in comparerTicket.MediaUrls)
                {
                    if (!MediaUrls.Contains(element))
                    {
                        falseDetected = true;
                    }
                }

                if(falseDetected == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}