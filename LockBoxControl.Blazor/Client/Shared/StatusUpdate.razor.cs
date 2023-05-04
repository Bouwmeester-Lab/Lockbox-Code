using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;

namespace LockBoxControl.Blazor.Client.Shared
{
    public partial class StatusUpdate : IDisposable
    {
        [Inject]
        public IStatusHubClient? StatusHubClient { get; set; }

        [Inject]
        public ICrudClient<ArduinoStatus, Guid>? StatusApiClient { get; set; }

        private bool isLoading = true;

        private ArduinoStatus? LatestStatus { get; set; }

        public void Dispose()
        {

            if (StatusHubClient != null)
            {
                StatusHubClient.StatusReceived -= StatusReceived;
            }
        }

        

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            ArgumentNullException.ThrowIfNull(StatusApiClient);

            // load from api the latest
            var results = (await StatusApiClient.GetPageAsync(1, 1).ConfigureAwait(false));

            if(results is not null && results.TotalMatches > 0)
            {
                LatestStatus = results.Results.First();
            }

           

            if (StatusHubClient != null)
            {
                // register

                StatusHubClient.StatusReceived += StatusReceived;

                // initialize hub client

                await StatusHubClient.InitializeAsync();
            }
            isLoading = false;


        }

        protected void StatusReceived(object? sender, ArduinoStatus status)
        {
            LatestStatus = status;
            StateHasChanged();
        }
    }
}
