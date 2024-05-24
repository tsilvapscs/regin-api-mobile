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
    public class InternalBasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                var token = authenticationToken.Replace("Bearer ", string.Empty);
                var decodedAuthentication = Cipher.Decrypt(token, Cipher.pass);

                var isValid = decodedAuthentication == "InTeRnAlPsCs@pR0s0L9t10N@";

                if (isValid)
                {
                    var principal = new GenericPrincipal(new GenericIdentity("PsCs"), null);
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