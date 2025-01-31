﻿using System;
using System.Collections.Generic;

namespace BugTracker.Models.Domain
{
    public class TicketType
    {
        public string Id { get; set; }
        public string TypeName { get; set; }

        public virtual List<Ticket> Tickets { get; set; }

        public TicketType()
        {
            Id = Guid.NewGuid().ToString();
            Tickets = new List<Ticket>();
        }
    }
}