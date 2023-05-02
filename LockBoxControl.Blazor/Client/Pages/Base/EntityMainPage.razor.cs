﻿using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LockBoxControl.Blazor.Client.Pages.Base
{
    [CascadingTypeParameter(nameof(T))]
    public partial class EntityMainPage<T, TId>
    {
        protected ObservableCollection<T> Items { get; set; } = new ObservableCollection<T>();

        [Parameter]
        public RenderFragment? OtherColumns { get; set; }

        [Parameter]
        public RenderFragment<T>? AddTemplate { get; set; }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public bool IsEditing { get; set; } = false;

        public bool IsLoading { get; set; } = true;

        public MudDataGrid<T>? DataGrid { get; set; }

        protected bool isAddDialogOpen = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadItemsAsync(1, PageSize);
        }

        protected void ClosePopup()
        {
            isAddDialogOpen = false;
            // reset the entity
            // Entity 
            ResetForm();
        }

        protected async Task DeleteAsync()
        {
            ArgumentNullException.ThrowIfNull(Client);

            var selected = DataGrid?.Selection;

            if(selected != null)
            {
                var tasks = new List<Task>();
                foreach(var item in selected)
                {
                    tasks.Add(DeleteItemAsync(item));
                }
                await Task.WhenAll(tasks);
                // clear selection
                DataGrid?.Selection.Clear();
            }
        }

        protected async Task DeleteItemAsync(T item)
        {
            ArgumentNullException.ThrowIfNull(Client);
            
            await Client.DeleteAsync(item.Id);

            // get rid of the local copy
            Items.Remove(item);
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

