using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BugTracker.Models.Filters
{
    public class MVCFiltersAuthorization : AuthorizeAttribute
    {
        // Override the default behavior when a user is not authenticated.
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if(filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Home" },
                        { "Action", "Unauthorized" }
                    });
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}