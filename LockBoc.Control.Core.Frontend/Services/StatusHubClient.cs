using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Models;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Frontend.Services
{
    public class StatusHubClient : IStatusHubClient
    {
        private readonly string hubPath = "Hubs/Status";

        private HubConnection? hubConnection;

        private readonly string baseUrl;

        public EventHandler<ArduinoStatus>? StatusReceived { get; set; }

        public StatusHubClient(IOptions<HubOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);
            ArgumentException.ThrowIfNullOrEmpty(options.Value?.BaseUrl, nameof(baseUrl));

            baseUrl = options.Value.BaseUrl.TrimEnd('/', '\\');
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            hubConnection = new HubConnectionBuilder().WithUrl($"{baseUrl}/{hubPath}").WithAutomaticReconnect().Build();

            hubConnection.On<ArduinoStatus>(nameof(IStatusClient.ReceiveStatus), status =>
            {
                StatusReceived?.Invoke(this, status);
            });

            await hubConnection.StartAsync(cancellationToken).ConfigureAwait(false);

        }
    }
}
