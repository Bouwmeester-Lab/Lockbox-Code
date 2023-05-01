using LockBoxControl.Core.Frontend.Models;
using Microsoft.AspNetCore.Components;

namespace LockBoxControl.Blazor.Client.Pages.Data.Arduinos
{
    public partial class EditArduino
    {
        public EditArduino() 
        {
            SubmitAction = SubmitActions.Update;
        }

        protected override async Task OnInitializedAsync()
        {
            ArgumentNullException.ThrowIfNull(Client);

            await base.OnInitializedAsync();

            // load the entity
            if(Entity is not null && Entity.Id != 0)
            {
                Entity = await Client.GetByIdAsync(Entity.Id);
                StateHasChanged();
            }
        }
    }
}
