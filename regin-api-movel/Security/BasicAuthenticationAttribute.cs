using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace regin_api_movel.Security
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                var token = authenticationToken.Replace("Bearer ", string.Empty);
                string decodedAuthentication = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var usernamePasswordArray = decodedAuthentication.Split(':');
                var userName = usernamePasswordArray[0];
                var password = usernamePasswordArray[1];

                var isValid = userName == "08584530967" && password == "Yl0I5beewIR4L4TsFTfgXI0mb18ohL2g";

                if (isValid)
                {
                    var principal = new GenericPrincipal(new GenericIdentity(userName), null);
                    Thread.CurrentPrincipal = principal;

                    return;                
                }
            }

            HandleUnathorized(actionContext);
        }

        private static void HandleUnathorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
    }
}