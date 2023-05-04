using LockBoxControl.Core.Models;

namespace LockBoxControl.Core.Frontend.Contracts
{
    public interface IStatusHubClient
    {
        EventHandler<ArduinoStatus>? StatusReceived { get; set; }

        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}