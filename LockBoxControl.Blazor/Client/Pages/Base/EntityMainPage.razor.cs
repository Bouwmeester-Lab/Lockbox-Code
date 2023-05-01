using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace LockBoxControl.Blazor.Client.Pages.Base
{
    [CascadingTypeParameter(nameof(T))]
    public partial class EntityMainPage<T, TId>
    {
        protected ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

        [Parameter]
        public RenderFragment? OtherColumns { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public bool IsEditing { get; set; } = false;

        public bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadItemsAsync(1, PageSize);
        }


        protected async Task LoadItemsAsync(int pageNumber, int pageSize = 10)
        {
            IsLoading = true;
            ArgumentNullException.ThrowIfNull(Client);
            var page = await Client.GetPageAsync(pageNumber, pageSize);

            if (page == null)
            {
                // error
                return;
            }

            PageNumber = page.PageNumber;
            TotalPages = page.TotalPages;

            // clear
            Items.Clear();

            foreach (var item in page.Results)
            {
                Items.Add(item);
            }
            IsLoading = false;
        }
    }
}

