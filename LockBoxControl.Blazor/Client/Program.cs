using LockBoxControl.Blazor.Client;
using LockBoxControl.Blazor.Client.Contracts;
using LockBoxControl.Blazor.Client.Extensions;
using LockBoxControl.Blazor.Client.Services;
using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Extensions;
using LockBoxControl.Core.Frontend.Services;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IThemeService, ThemeService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddApiClient<Arduino>(builder.Configuration);
builder.Services.AddApiClient<Command>(builder.Configuration);
builder.Services.AddApiClient<ArduinoStatus, Guid, ArduinoStatusesClient>(builder.Configuration);
builder.Services.AddApiClient<Request, Guid>(builder.Configuration);
builder.Services.AddRunCommandClient(builder.Configuration);

builder.Services.ConfigureHubClient(builder.Configuration);

builder.Services.AddMudServices();

await builder.Build().RunAsync();