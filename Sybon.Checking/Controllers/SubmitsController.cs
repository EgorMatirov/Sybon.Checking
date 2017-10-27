using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Auth.Client.Api;
using Sybon.Checking.Services.SubmitResultService;
using Sybon.Checking.Services.SubmitResultService.Models;
using Sybon.Checking.Services.SubmitService;
using Sybon.Checking.Services.SubmitService.Models;

namespace Sybon.Checking.Controllers
{
    [Route("api/[controller]")]
    public class SubmitsController : Controller, ILogged
    {
        private const int RequestsLimitExceededStatusCode = 429;

        [HttpPost("send")]
        [SwaggerOperation("Send")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> Send(
            [FromServices] IPermissionsApi permissionsApi,
            [FromServices] ISubmitService submitService,
            [FromBody] Submit submit)
        {
            var limitExceeded = !permissionsApi.TryIncreaseRequestsCountBy(UserId, 1);
            if(limitExceeded == null || limitExceeded.Value) return new StatusCodeResult(RequestsLimitExceededStatusCode);

            var permission = permissionsApi.GetToProblem(UserId, submit.ProblemId);
            if (!permission.Contains("Read"))
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            submit.Created = DateTime.UtcNow;
            submit.UserId = UserId;
            var submitId = await submitService.SendAsync(submit);
            return Ok(submitId);
        }

//        [HttpPost("sendall")]
//        [SwaggerOperation("SendAll")]
//        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ICollection<long>))]
//        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
//        [AuthorizeFilter]
//        public async Task<IActionResult> SendAll([FromBody] Submit[] submits)
//        {
//            var token = ConfigManager.Configuration["sybon:auth:sysuser:token"];
//            var permissionsApi =
//                new PermissionsApi(new Configuration(apiKey: new Dictionary<string, string>() {{"api_key", token}}));
//            var limitExceeded = !permissionsApi.TryIncreaseRequestsCountBy(UserId, submits.Length);
//            if (limitExceeded == null || limitExceeded.Value) return new HttpStatusCodeResult(RequestsLimitExceededStatusCode);
//            bool authorized = submits.All(x =>
//            {
//                var permission = permissionsApi.GetToProblem(UserId, x.ProblemId);
//                return permission.Contains("Read");
//            });
//            if (!authorized)
//                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);
//            var submitIds = new long[submits.Length];
//            for (int i = 0; i < submits.Length; ++i)
//            {
//                submitIds[i] = await ServiceUoW.SubmitService.Send(submits[i]);
//            }
//            return Ok(submitIds);
//        }
//
//        [HttpPost("rejudge")]
//        [SwaggerOperation("Rejudge")]
//        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
//        [AuthorizeFilter]
//        public IActionResult Rejudge([FromBody] long[] ids)
//        {
//            var token = ConfigManager.Configuration["sybon:auth:sysuser:token"];
//            var permissionsApi =
//                new PermissionsApi(new Configuration(apiKey: new Dictionary<string, string>() { { "api_key", token } }));
//            var limitExceeded = !permissionsApi.TryIncreaseRequestsCountBy(UserId, ids.Length);
//            if (limitExceeded == null || limitExceeded.Value) return new HttpStatusCodeResult(RequestsLimitExceededStatusCode);
//            bool authorized = ids.All(x =>
//            {
//                var submit = ServiceUoW.SubmitService.Get(x);
//                var permission = permissionsApi.GetToProblem(UserId, submit.ProblemId);
//                return permission.Contains("Read");
//            });
//            if (!authorized)
//                return new HttpStatusCodeResult((int) HttpStatusCode.Unauthorized);
//            foreach (var id in ids)
//            {
//                ServiceUoW.SubmitService.Rejudge(id);
//            }
//            return Ok();
//        }
//
        [HttpGet("results")]
        [SwaggerOperation("GetSubmitResults")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<SubmitResult>))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public IActionResult GetSubmitResults([FromServices] ISubmitResultService submitResultService, [FromServices] ISubmitService submitService, [FromQuery] string ids)
        {
            var idList = ids.Split(",");
            //TODO: check auth
//            var permissionsApi = new PermissionsApi(new Configuration(apiKey: ApiKeyDict));
//
//            bool authorized = idList.All(x =>
//            {
//                var submit = ServiceUoW.SubmitService.Get(x);
//                var permission = permissionsApi.GetToProblem(UserId, submit.ProblemId);
//                return permission.Contains("Read");
//            });
//            if (!authorized)
//                return new HttpStatusCodeResult((int)HttpStatusCode.Unauthorized);

            var result = idList
                .Select(int.Parse)
                .Select(async x => await submitService.GetAsync(x))
                .Select(x => x.Result.Result)
                .ToArray();
            return Ok(result);
        }
        
        public long UserId { get; set; }

        public string ApiKey { get; set; }
    }
}