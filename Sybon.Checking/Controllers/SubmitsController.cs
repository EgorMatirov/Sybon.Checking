using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Auth.Client.Api;
using Sybon.Checking.Repositories.SubmitResultRepository;
using Sybon.Checking.Services.SubmitResultService;
using Sybon.Checking.Services.SubmitService;
using Sybon.Checking.Services.SubmitService.Models;
using SubmitResult = Sybon.Checking.Services.SubmitResultService.Models.SubmitResult;
using Sybon.Common;

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
            [FromServices] IMapper mapper,
            [FromBody] SubmitModel submitModel)
        {
            var limitExceeded = !permissionsApi.TryIncreaseRequestsCountBy(UserId, 1);
            if(limitExceeded == null || limitExceeded.Value) return new StatusCodeResult(RequestsLimitExceededStatusCode);

            var permission = permissionsApi.GetToProblem(UserId, submitModel.ProblemId);
            if (!permission.Contains("Read"))
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            var submit = mapper.Map<Submit>(submitModel);
            submit.Created = DateTime.UtcNow;
            submit.UserId = UserId;
            var submitId = await submitService.SendAsync(submit);
            return Ok(submitId);
        }

        [HttpPost("sendall")]
        [SwaggerOperation("SendAll")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<long>))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> SendAll(
            [FromServices] IPermissionsApi permissionsApi,
            [FromServices] ISubmitService submitService,
            [FromServices] IMapper mapper,
            [FromBody] SubmitModel[] submitModels)
        {
            var limitExceeded = !permissionsApi.TryIncreaseRequestsCountBy(UserId, submitModels.Length);
            if(limitExceeded == null || limitExceeded.Value) return new StatusCodeResult(RequestsLimitExceededStatusCode);

            var permissions = await permissionsApi.GetToProblemsAsync(UserId, string.Join(",", submitModels.Select(x => x.ProblemId)));
            if (!permissions.All(permission => permission.Contains("Read")))
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);

            var submits = mapper.Map<Submit[]>(submitModels);
            var submitCreated = DateTime.UtcNow;
            foreach (var submit in submits)
            {
                submit.Created = submitCreated;
                submit.UserId = UserId;
            }
            var submitIds = new long[submits.Length];
            for (var i = 0; i<submits.Length; ++i)
            {
                submitIds[i] = await submitService.SendAsync(submits[i]);
            }
            return Ok(submitIds);
        }

        [HttpPost("rejudge")]
        [SwaggerOperation("Rejudge")]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> Rejudge(
            [FromServices] IPermissionsApi permissionsApi,
            [FromServices] ISubmitService submitService,
            [FromBody] long[] ids)
        {
            var limitExceeded = !permissionsApi.TryIncreaseRequestsCountBy(UserId, ids.Length);
            if(limitExceeded == null || limitExceeded.Value) return new StatusCodeResult(RequestsLimitExceededStatusCode);

            var submits = await submitService.GetAllAsync(ids);
            
            var permissions = await permissionsApi.GetToProblemsAsync(UserId, string.Join(",", submits.Select(x => x.ProblemId)));
            if (!permissions.All(permission => permission.Contains("Read")))
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            
            foreach (var id in ids)
            {
                await submitService.RejudgeAsync(id);
            }
            return Ok();
        }

        [HttpGet("results")]
        [SwaggerOperation("GetSubmitResults")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<SubmitResult>))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> GetSubmitResults(
            [FromServices] IPermissionsApi permissionsApi,
            [FromServices] ISubmitService submitService,
            [FromServices] ISubmitResultService submiResultService,
            [FromQuery] string ids)
        {
            var idList = ids.Split(",").Select(long.Parse).ToArray();
            var submits = await submitService.GetAllAsync(idList);

            var permissions = await permissionsApi.GetToProblemsAsync(UserId, string.Join(",", submits.Select(x => x.ProblemId)));
            if (!permissions.All(permission => permission.Contains("Read")))
                return new StatusCodeResult((int)HttpStatusCode.Unauthorized);
            
            var result = await submiResultService.GetAllBySubmitIdsAsync(idList);
            foreach (var submitResult in result)
            {
                foreach (var testGroupResult in submitResult.TestGroupResults)
                {
                    testGroupResult.TestResults = testGroupResult.TestResults
                            .Where(x => x.Status != TestResultStatus.SKIPPED)
                            .ToArray();
                }
            }
            return Ok(result);
        }
        
        public class SubmitModel
        {
            public long CompilerId { get; set; }
            public byte[] Solution { get; set; }
            public SolutionFileType SolutionFileType { get; set; }
            public long ProblemId { get; set; }
            public bool PretestsOnly { get; set; }
            public ContinueCondition ContinueCondition { get; set; }
        }
        
        public long UserId { get; set; }
        public string ApiKey { get; set; }
    }
}