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
        public async Task RegisterMacAddressAsync([Required] string macAddress, CancellationToken cancellationToken = default)
        {
            await pingManager.RegisterArduinoAsync(macAddress, cancellationToken);
        }

        [HttpPost]
        public async Task<ActionResult<ArduinoStatus?>> UpdateStatusAsync([FromBody] ArduinoStatusDTO arduinoStatus, CancellationToken cancellationToken = default)
        {
            return Ok(await pingManager.UpdateStatusAsync(arduinoStatus, cancellationToken));
        }
    }
}
