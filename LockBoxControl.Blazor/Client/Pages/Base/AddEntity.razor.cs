using LockBoxControl.Core.Frontend.Models;

namespace LockBoxControl.Blazor.Client.Pages.Base
{
    public partial class AddEntity<TEntity, TId>
    {
        public AddEntity()
        {
            ResetForm();
            SubmitAction = SubmitActions.Create;
        }
    }
}
