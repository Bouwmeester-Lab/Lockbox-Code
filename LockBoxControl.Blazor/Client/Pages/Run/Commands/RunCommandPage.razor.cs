using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Services;
using LockBoxControl.Core.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace LockBoxControl.Blazor.Client.Pages.Run.Commands;

public partial class RunCommandPage
{
    [Inject]
    public ICrudClient<Command, long>? CommandClient { get; private set; }
    [Inject]
    public ICrudClient<Arduino, long>? ArduinoClient { get; private set; }

    public List<Arduino> Arduinos { get; private set; } = new List<Arduino>();
    public List<Command> Commands { get; private set; } = new List<Command>();

    protected override async Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(ArduinoClient);
        ArgumentNullException.ThrowIfNull(CommandClient);

        var arduinos = await ArduinoClient.GetAllAsync();
        if(arduinos is not null )
        {
            foreach (var arduino in arduinos)
            {
                Arduinos.Add(arduino);
            }
        }

        var commands = await CommandClient.GetAllAsync();
        if(commands is not null )
        {
            foreach (var command in commands)
            {
                Commands.Add(command);
            }
        }
        
    }
}
