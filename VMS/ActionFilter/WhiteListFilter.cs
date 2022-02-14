using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VMS.Models;

namespace VMS.ActionFilter
{
    public class WhiteListFilter : Attribute, IActionFilter
    {
        private VMSDbContext _context = new VMSDbContext();
      
        public void OnActionExecuting(ActionExecutingContext context)
        {

            string ip = string.Empty;
            if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["X-Forwarded-For"]))
            {
                ip = context.HttpContext.Request.Headers["X-Forwarded-For"];
                var checkIP = _context.WhiteListIpaddresses.Where(x => x.Ipaddress == ip).FirstOrDefault();
                if (checkIP == null) {

                    var values = new RouteValueDictionary(new
                    {
                        action = "UnAuthorized",
                        controller = "Appointment",
                        code = "1"
                    });
                    context.Result = new RedirectToRouteResult(values);

                }

            }
            else
            {
                ip = context.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
            }

         

        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
 
        }

     
    }
}
