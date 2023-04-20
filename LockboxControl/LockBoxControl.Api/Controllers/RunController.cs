using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using LockboxControl.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LockBoxControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunController : ControllerBase
    {
        private readonly PortManager portManager;
        private readonly IQueryableRepositoryService<Command> commandService;

        public RunController(PortManager portManager, IQueryableRepositoryService<Command> commandService)
        {
            this.portManager = portManager;
            this.commandService = commandService;
        }

        [HttpPost("{commandId}")]
        public async Task<ActionResult> RunAsync(long commandId, [FromQuery] long arduinoId = 0, CancellationToken cancellationToken = default)
        {
            var command = await commandService.GetAsync(commandId, cancellationToken);
            if (command is null)
            {
                return NotFound("The command doesn't exist.");
            }

            if (arduinoId != 0) 
            {
                await portManager.SendCommandAsync(arduinoId, command, cancellationToken);
            }
            else
            {
                await portManager.SendCommandAsync(command, cancellationToken);
            }
            return Ok();
        }

        
    }
}
