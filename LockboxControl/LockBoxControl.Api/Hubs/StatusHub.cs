using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace LockBoxControl.Api.Hubs
{
    public class StatusHub : Hub<IStatusClient>
    {
        public async Task SendStatus(ArduinoStatus status)
        {
            await Clients.All.ReceiveStatus(status);
        }
    }
}
