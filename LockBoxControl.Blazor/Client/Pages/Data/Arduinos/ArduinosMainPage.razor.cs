using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace LockBoxControl.Blazor.Client.Pages.Data.Arduinos
{
    public partial class ArduinosMainPage
    {

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        public bool IsEditing { get; set; } = false;
        

        protected ObservableCollection<Arduino> Arduinos { get; set; } = new ObservableCollection<Arduino>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadArduinosAsync(1, PageSize);
        }

        public void StartedEditingItem(Arduino item)
        {
            Entity = item;
            IsEditing = true;
            SubmitAction = Core.Frontend.Models.SubmitActions.Update;
        }

        


        protected async Task LoadArduinosAsync(int pageNumber, int pageSize = 10)
        {
            ArgumentNullException.ThrowIfNull(Client);
            var page = await Client.GetPageAsync(pageNumber, pageSize);

            if(page == null)
            {
                // error
                return;
            }

            PageNumber = page.PageNumber;
            TotalPages = page.TotalPages;

            // clear
            Arduinos.Clear();

            foreach(var arduino in page.Results)
            {
                Arduinos.Add(arduino);
            }
        }
    }
}
