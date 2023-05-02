using LockBoxControl.Blazor.Client.Resources;
using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Models;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace LockBoxControl.Blazor.Client.Pages.Base
{
    public partial class EditEntity<TEntity, TId>
        where TEntity : IEntity<TId>
        where TId : IEquatable<TId>
    {
        [CascadingParameter] public EntityMainPage<TEntity, TId>? MainPage { get; set; }

        [Inject]
        protected ICrudClient<TEntity, TId>? Client { get; set; }

        [Inject]
        protected ISnackbar? Snackbar { get; set; }

        [Parameter]
        public TEntity? Entity { get; set; }

        [Parameter]
        public bool IsFinished { get; set; } = false;

        [Parameter]
        public EventCallback<TEntity> ItemCreated { get; set; }

        protected bool success;

        public EditEntity()
        {
            ResetForm();
        }

        public virtual void ResetForm()
        {

        }

        public virtual async Task CreateNewAsync(CancellationToken cancellationToken = default)
        {
            if(Entity is not null && Client is not null)
            {
                var entity = await Client.SaveAsync(Entity, cancellationToken);
                
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

                if(entity is not null)
                {
                    Snackbar.Add(string.Format(Strings.SuccessCreate, typeof(TEntity).Name, entity.Id), Severity.Success);
                    MainPage?.AddItem(entity);
                    await ItemCreated.InvokeAsync(entity);
                }
                else
                {
                    Snackbar.Add(string.Format(Strings.FailedCreate, typeof(TEntity).Name), Severity.Error);
                }

            }
        }

        public virtual async Task UpdateAsync(CancellationToken cancellationToken = default)
        {
            if (Entity is not null && Client is not null)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
                try
                {
                    await Client.UpdateAsync(Entity, cancellationToken);
                    Snackbar.Add(string.Format(Strings.SuccessUpdate, typeof(TEntity).Name, Entity.Id), Severity.Success);
                }
                catch(HttpRequestException ex)
                {
                    Snackbar.Add(string.Format(Strings.FailedUpdate, typeof(TEntity).Name, Entity.Id, ex.Message), Severity.Error);
                }
            }
        }


        [Parameter]
        public SubmitActions SubmitAction { get; set; }

        public virtual async Task SubmitAsync(EditContext context)
        {
            await SubmitAsyncNoContext();
        }

        public virtual async Task SubmitAsyncNoContext()
        {
            IsFinished = false;
            await (SubmitAction switch
            {
                SubmitActions.Update => UpdateAsync(),
                SubmitActions.Create => CreateNewAsync(),
                _ => Task.CompletedTask,
            });
            IsFinished = true;
        }

        public async Task CommitChangesAsync(TEntity item)
        {
            Entity = item;
            await SubmitAsyncNoContext();
        }

    }
}
