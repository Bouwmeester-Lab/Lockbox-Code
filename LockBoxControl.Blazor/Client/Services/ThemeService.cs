using LockBoxControl.Blazor.Client.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LockBoxControl.Blazor.Client.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IJSRuntime jsRuntime;
        private readonly DotNetObjectReference<ThemeService> _dotNetRef;

        public List<EventCallback<Theme>> ThemeChangedCallbacks { get; set; } = new List<EventCallback<Theme>>();

        public ThemeService(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            this.jsRuntime = jsRuntime;
        }

        public async Task SetCurrentThemeAsync(Theme theme)
        {
            var tasks = new List<Task>();
            foreach (var callback in ThemeChangedCallbacks)
            {
                tasks.Add(callback.InvokeAsync(theme));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<Theme> GetCurrentThemeAsync()
        {
            return await jsRuntime.InvokeAsync<bool>("darkModeChange", _dotNetRef) switch
            {
                true => Theme.Dark,
                false => Theme.Light,
            };
        }
    }

    public enum Theme
    {
        Dark,
        Light,
    }
}
