using LockBoxControl.Blazor.Client;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Services;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<ICrudClient<Arduino, long>, CrudClient<Arduino, long>>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BaseApiAddress"]
        ?? throw new ArgumentNullException("BaseApiAddress"));
});


builder.Services.AddMudServices();

await builder.Build().RunAsync();