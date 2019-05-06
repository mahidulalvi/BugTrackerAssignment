using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.Filters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        //After action executes
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        //Before action executes
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = new ApplicationDbContext();

            var log = new ActionLog();

            log.ActionName = filterContext
                .ActionDescriptor
                .ActionName;

            log.ControllerName = filterContext
                .ActionDescriptor
                .ControllerDescriptor
                .ControllerName;

            context.ActionLogs.Add(log);
            context.SaveChanges();
        }

        //After ViewResult executes.In other words, after rendering the view.
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }

        //Before ViewResult executes. In other words, before rendering the view.
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }
    }
}