namespace LockBoxControl.Blazor.Client.Pages.Data.Commands
{
    public partial class CommandMainPage
    {
        public override void ResetForm()
        {
            Entity = new Core.Models.Command
            {
                CommandLetter = "",
                Description = "",
                Id = 0,
                Name = ""
            };
        }
    }
}
