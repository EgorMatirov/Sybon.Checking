using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sybon.Auth.Client.Api;

namespace Sybon.Checking
{
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {
        private readonly bool _adminOnly;

        public AuthorizeFilterAttribute(bool adminOnly = false)
        {
            _adminOnly = adminOnly;
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var apiKey = context.HttpContext.Request.Query["api_key"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var accountApi = (IAccountApi) context.HttpContext.RequestServices.GetService(typeof(IAccountApi));
            var tokenResponse = accountApi.CheckTokenAsyncWithHttpInfo(apiKey).Result;
            if (tokenResponse.StatusCode != (int)HttpStatusCode.OK || tokenResponse.Data == null || tokenResponse.Data.ExpiresIn != null && tokenResponse.Data.ExpiresIn <= 0)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!(context.Controller is ILogged controller))
                throw new Exception("Controller must derive from ILogged");
            Debug.Assert(tokenResponse.Data.UserId != null, "token.UserId != null");
            controller.UserId = tokenResponse.Data.UserId.Value;
            controller.ApiKey = tokenResponse.Data.Key;

            if (_adminOnly)
            {
                var permissionsApi = (IPermissionsApi) context.HttpContext.RequestServices.GetService(typeof(IPermissionsApi));
                var userRole = permissionsApi.GetUserRole(controller.UserId);
                if (userRole != "\"Admin\"")
                    context.Result = new UnauthorizedResult();
            }
        }
    }

    public interface ILogged
    {
        long UserId { get; set; }
        string ApiKey { get; set; }
    }
}