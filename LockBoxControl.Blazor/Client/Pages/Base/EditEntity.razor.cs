using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace LockBoxControl.Blazor.Client.Pages.Base
{
    public partial class EditEntity<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        [Inject]
        protected ICrudClient<TEntity, TId>? Client { get; set; }

        [Parameter]
        public TEntity? Entity { get; set; }

        public EditEntity()
        {
        }

        public virtual Task CreateNewAsync(CancellationToken cancellationToken = default)
        {
            if(Entity is not null)
                return Client?.SaveAsync(Entity, cancellationToken) ?? Task.CompletedTask;
            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(CancellationToken cancellationToken = default)
        {
            if (Entity is not null)
                return Client?.UpdateAsync(Entity, cancellationToken) ?? Task.CompletedTask; 
            return Task.CompletedTask;
        }


        [Parameter]
        public SubmitActions SubmitAction { get; set; }

        public virtual Task SubmitAsync(EditContext context)
        {
            return SubmitAction switch
            {
                SubmitActions.Update => UpdateAsync(),
                SubmitActions.Create => CreateNewAsync(),
                _ => Task.CompletedTask,
            };
        }

    }
}
