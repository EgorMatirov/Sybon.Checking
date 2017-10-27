using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Checking.Services.CompilersService;
using Sybon.Checking.Services.CompilersService.Models;

namespace Sybon.Checking.Controllers
{
    [Route("api/[controller]")]
    public class CompilersController : Controller
    {
        [HttpGet]
        [SwaggerOperation("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<Compiler>))]
        public async Task<IActionResult> Get([FromServices] ICompilersService compilersService)
        {
            var compilers = await compilersService.GetAllAsync();
            return Ok(compilers);
        }
    }
}