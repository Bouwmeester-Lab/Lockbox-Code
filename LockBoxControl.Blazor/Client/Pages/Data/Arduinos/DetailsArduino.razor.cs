using LockBoxControl.Blazor.Client.Pages.Base;
using LockBoxControl.Core.Frontend.Models;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;

namespace LockBoxControl.Blazor.Client.Pages.Data.Arduinos
{
    public partial class DetailsArduino : EditEntity<Arduino, long>
    {
        [Parameter]
        public bool ShowId { get; set; }

        
    }
}
