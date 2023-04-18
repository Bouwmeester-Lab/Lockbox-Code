using LockboxControl.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        [HttpPost]
        public async Task RunAsync([FromBody] Command command)
        {
            
        }
    }
}
