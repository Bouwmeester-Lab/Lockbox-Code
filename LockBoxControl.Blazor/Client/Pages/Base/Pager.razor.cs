using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor;

namespace LockBoxControl.Blazor.Client.Pages.Base
{
    public partial class Pager<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter] public MudDataGrid<T>? DataGrid { get; set; }

        [Parameter] public EventCallback<int> RowsPerPageChanged { get; set; }
        [Parameter] public EventCallback OnFirstPageRequested { get; set; }
        /// <summary>
        /// Used when a new page is requested.
        /// </summary>
        [Parameter] public EventCallback<int> PageChangedRequested { get; set; }

        /// <summary>
        /// Set true to hide the part of the pager which allows to change the page size.
        /// </summary>
        [Parameter] public bool DisableRowsPerPage { get; set; }

        /// <summary>
        /// Set true to disable user interaction with the backward/forward buttons
        /// and the part of the pager which allows to change the page size.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Define a list of available page size options for the user to choose from
        /// </summary>
        [Parameter] public int[] PageSizeOptions { get; set; } = new int[] { 10, 25, 50, 100 };

        /// <summary>
        /// Format string for the display of the current page, which you can localize to your language. Available variables are:
        /// {first_item}, {last_item} and {all_items} which will replaced with the indices of the page's first and last item, as well as the total number of items.
        /// Default: "{first_item}-{last_item} of {all_items}" which is transformed into "0-25 of 77". 
        /// </summary>
        [Parameter] public string InfoFormat { get; set; } = "{0}-{1} of {2}";

        [Parameter] public int TotalElements { get; set; } = 0;
        [Parameter] public int CurrentPage { get; set; } = 1;
        [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }

        /// <summary>
        /// The localizable "Rows per page:" text.
        /// </summary>
        [Parameter] public string RowsPerPageString { get; set; } = "Rows per page:";

        private string Info => DataGrid == null ? "DataGrid==null" : string.Format(InfoFormat, (CurrentPage - 1) * DataGrid.RowsPerPage + 1, Math.Min(CurrentPage * DataGrid.RowsPerPage, TotalElements), TotalElements);
            
        //DataGrid == null ? "DataGrid==null" : InfoFormat
            //.Replace("{first_item}", $"{DataGrid?.CurrentPage * DataGrid.RowsPerPage + 1}")
            //.Replace("{last_item}", $"{Math.Min((DataGrid.CurrentPage + 1) * DataGrid.RowsPerPage, DataGrid.GetFilteredItemsCount())}")
            //.Replace("{all_items}", $"{DataGrid.GetFilteredItemsCount()}");

        private bool BackButtonsDisabled => Disabled || (DataGrid == null ? false : CurrentPage == 1);

        private bool ForwardButtonsDisabled => Disabled || (DataGrid == null ? false : CurrentPage * DataGrid.RowsPerPage >= TotalElements);

        protected string Classname =>
            new CssBuilder("mud-table-pagination-toolbar")
            .AddClass(Class)
            .Build();

        private async Task SetRowsPerPageAsync(int size)
        {
            if(DataGrid is not null)
                await DataGrid.SetRowsPerPageAsync(size);
            await RowsPerPageChanged.InvokeAsync(size);

        }

        private async Task IncrementPageAsync()
        {
            CurrentPage += 1;
            await PageChangedRequested.InvokeAsync(CurrentPage);
        }

        private async Task DecreasePageAsync()
        {
            CurrentPage -= 1;
            await PageChangedRequested.InvokeAsync(CurrentPage);
        }

        private async Task GoToLastPageAsync()
        {
            if(DataGrid != null)
            {
                if(TotalElements % DataGrid.RowsPerPage == 0)
                {
                    CurrentPage = TotalElements / DataGrid.RowsPerPage;
                }
                else
                {
                    CurrentPage = TotalElements / DataGrid.RowsPerPage + 1;
                }

                await PageChangedRequested.InvokeAsync(CurrentPage);
            }
            
        }

        protected override async Task OnInitializedAsync()
        {
            //if (DataGrid != null)
            //{
            //    DataGrid.HasPager = true;
            //    DataGrid.PagerStateHasChangedEvent += StateHasChanged;
            //    var size = DataGrid._rowsPerPage ?? PageSizeOptions.First();
            //    await DataGrid.SetRowsPerPageAsync(size);
            //}
        }

        public void Dispose()
        {
            //if (DataGrid != null)
            //{
            //    DataGrid.PagerStateHasChangedEvent -= StateHasChanged;
            //}
        }
    }
}
