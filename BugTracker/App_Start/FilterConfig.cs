﻿using BugTracker.Models.Filters;
using System.Web.Mvc;

namespace BugTracker
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogActionFilter());
        }
    }
}
