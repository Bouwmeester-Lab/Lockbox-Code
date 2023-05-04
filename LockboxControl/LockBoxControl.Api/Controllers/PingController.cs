using LockBoxControl.Api.Hubs;
using LockBoxControl.Core.Backend.Services;
using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;
using LockBoxControl.Core.Models.ApiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace LockBoxControl.Api.Controllers
{
    /// <summary>
    /// A controller meant for status updates from different arduinos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly PingManager pingManager;
        private readonly IHubContext<StatusHub, IStatusClient> hubContext;
        private readonly IQueryableRepositoryService<Arduino> arduinoService;
        public PingController(PingManager pingManager, IHubContext<StatusHub, IStatusClient> hubContext, IQueryableRepositoryService<Arduino> arduinoService)
        {
            this.pingManager = pingManager;
            this.hubContext = hubContext;
            this.arduinoService = arduinoService;
        }
        [HttpPost("{macAddress}")]
        public async Task<ActionResult> RegisterMacAddressAsync([Required] string macAddress, CancellationToken cancellationToken = default)
        {
            if(await pingManager.RegisterArduinoAsync(macAddress, cancellationToken))
            {
                return Ok();
            }
            return NotFound("The mac address wasn't found or the arduino serial ports are busy.");
        }

        [HttpPost]
        public async Task<ActionResult<ArduinoStatus?>> UpdateStatusAsync([FromBody] ArduinoStatusDTO arduinoStatus, CancellationToken cancellationToken = default)
        {
            var status = await pingManager.UpdateStatusAsync(arduinoStatus, cancellationToken);
            if(status is not null)
            {
                var arduino = await arduinoService.GetAsync(status.ArduinoId, cancellationToken);
                if(arduino is not null)
                {
                    status.Arduino = arduino;
                    await hubContext.Clients.All.ReceiveStatus(status);
                }
            }
            return status;
        }
    }
}
