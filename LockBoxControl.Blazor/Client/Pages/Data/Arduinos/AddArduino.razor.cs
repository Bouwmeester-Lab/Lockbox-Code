using LockBoxControl.Blazor.Client.Pages.Base;
using LockBoxControl.Core.Frontend.Models;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;

namespace LockBoxControl.Blazor.Client.Pages.Data.Arduinos
{
    public partial class AddArduino : EditEntity<Arduino, long>
    {
        

        public AddArduino() 
        {
            Entity = new Arduino
            {
                Description = "",
                Name = "",
                Id = 0,
                IsEnabled = true,
                PortName = ""
            };
            SubmitAction = SubmitActions.Create;
        }

        public override void ResetForm()
        {
            Entity = new Arduino
            {
                Description = "",
                Name = "",
                Id = 0,
                IsEnabled = true,
                PortName = ""
            };
        }
    }
}
