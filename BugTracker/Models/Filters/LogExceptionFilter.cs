using System.Web.Mvc;

namespace BugTracker.Models.Filters
{
    public class LogExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            //throw new NotImplementedException();
            var log = new ExceptionLog();
            log.Message = filterContext.Exception.Message;

            var dbcontext = new ApplicationDbContext();

            dbcontext.ExceptionLogs.Add(log);
            dbcontext.SaveChanges();

            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error"
            };
        }
    }
}