using LockBoxControl.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace LockBoxControl.Blazor.Client.Contracts
{
    public interface IThemeService
    {
        List<EventCallback<Theme>> ThemeChangedCallbacks { get; }

        Task SetCurrentThemeAsync(Theme theme);
        Task<Theme> GetCurrentThemeAsync();
    }
}
