using LockBoxControl.Blazor.Client.Pages.Base;
using LockBoxControl.Core.Frontend.Models;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;

namespace LockBoxControl.Blazor.Client.Pages.Data.Commands
{
    public partial class AddCommand : AddEntity<Command, long>
    {
        public override void ResetForm()
        {
            Entity = new Command
            {
                Description = "",
                Name = "",
                Id = 0,
                CommandLetter = ""
            };
        }
    }
}
