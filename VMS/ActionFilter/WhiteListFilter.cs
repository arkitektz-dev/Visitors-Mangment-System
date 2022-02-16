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

            var checkParamToken = context.ActionArguments.Count == 0 ? null : context.ActionArguments["Id"].ToString();
            if (checkParamToken != null) {

                //verifyToken
                var checkQr = _context.GeneratedTokens.Where(x => x.TokenNumber == checkParamToken).FirstOrDefault();
                if (checkQr != null)
                {
                    if (checkQr.IsUsed == true) {

                        var values = new RouteValueDictionary(new
                        {
                            action = "UnAuthorized",
                            controller = "Appointment",
                            code = "1"
                        });
                        context.Result = new RedirectToRouteResult(values);
                    }
   
                }


                return;
            }


            string ip = string.Empty;
            if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["X-Forwarded-For"]))
            {
                var ipAddress = context.HttpContext.Request.Headers["X-Forwarded-For"];
                ip = ipAddress.ToString().Split(':')[0];


                var checkIP = _context.WhiteListIpaddresses.Where(x => x.Ipaddress == ip).FirstOrDefault();
                if (checkIP == null)
                {


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
