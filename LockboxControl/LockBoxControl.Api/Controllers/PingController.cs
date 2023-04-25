using LockBoxControl.Core.Backend.Services;
using LockBoxControl.Core.Models;
using LockBoxControl.Core.Models.ApiDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public PingController(PingManager pingManager)
        {
            this.pingManager = pingManager;
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
            return Ok(await pingManager.UpdateStatusAsync(arduinoStatus, cancellationToken));
        }
    }
}
