using LockBoxControl.Core.Models;

namespace LockBoxControl.Core.Contracts
{
    public interface IStatusClient
    {
        Task ReceiveStatus(ArduinoStatus status);
    }
}
