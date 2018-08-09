using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoadBalance.Auth
{

    public class RpcAuth : ActionFilterAttribute  
    {
        public override void OnActionExecuting(ActionExecutingContext context) {
            base.OnActionExecuting(context);

            switch (Startup.GetConfig<string>("Auth.Type")) {
                case "Token":
                    OnTokenAuth(context);
                    break;
                case "Ip":
                    OnIpAuth(context);
                    break;
            }

            
            return;
        }

        private void OnIpAuth(ActionExecutingContext context) {

            var ip = GetUserIp(context.HttpContext);

            var configip = Startup.GetConfig<string>("Auth.AllowIp").Split("|");

            if (!configip.Contains(ip)) {

                context.Result = new UnauthorizedResult();
                return;

            }
        }

        private void OnTokenAuth(ActionExecutingContext context) {
            var header = context.HttpContext.Request.Headers["auth"];
            if (header.Count == 0) {
                context.Result = new UnauthorizedResult();
                return;
            }

            var auth = header.First();

            if (!auth.Equals("pass")) {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        public static string GetUserIp(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

    }
}
