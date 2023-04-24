using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Controllers
{
    /// <summary>
    /// A controller meant for status updates from different arduinos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
    }
}
