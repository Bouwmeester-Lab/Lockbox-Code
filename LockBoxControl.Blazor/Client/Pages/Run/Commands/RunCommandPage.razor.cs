using LockBoxControl.Core.Frontend.Contracts;
using LockBoxControl.Core.Frontend.Services;
using LockBoxControl.Core.Models;
using LockBoxControl.Core.Models.ApiDTO;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.ObjectModel;

namespace LockBoxControl.Blazor.Client.Pages.Run.Commands;

public partial class RunCommandPage
{
    [Inject]
    public ICrudClient<Command, long>? CommandClient { get; private set; }
    [Inject]
    public ICrudClient<Arduino, long>? ArduinoClient { get; private set; }
    [Inject]
    public RunCommandClient? RunCommandClient { get; private set; }
    [Inject]
    public ISnackbar? Snackbar { get; private set; }

    public List<Arduino> Arduinos { get; private set; } = new List<Arduino>();
    public List<Command> Commands { get; private set; } = new List<Command>();

    public List<Arduino> SelectedArduinos { get; set; } = new List<Arduino>();
    public Command? SelectedCommand { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ArgumentNullException.ThrowIfNull(ArduinoClient);
        ArgumentNullException.ThrowIfNull(CommandClient);
        ArgumentNullException.ThrowIfNull(RunCommandClient);

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
    private async Task RunCommandAsync()
    {
        ArgumentNullException.ThrowIfNull(RunCommandClient);
        // if no command is selected do nothing and show an error using snackbar
        if(SelectedCommand is null || SelectedCommand.Id == 0)
        {
            Snackbar?.Add(Resources.Strings.NoCommandSelected, Severity.Error);
            return;
        }

        // if the selected is empty or equal the # of arduinos let's optimize and use only one http call
        if(!SelectedArduinos.Any() || SelectedArduinos.Count == Arduinos.Count)
        {
            var statuses = await RunCommandClient.RunCommandAsync(SelectedCommand);
            ProcessStatuses(statuses);
        }
        else
        {
            var statuses = await RunCommandClient.RunCommandAsync(SelectedCommand, SelectedArduinos.Select(x => x.Id).ToArray());
            ProcessStatuses(statuses);
        }
    }

    private void ProcessStatuses(List<ArduinoCommandStatus>? statuses)
    {
        if (statuses is null)
        {
            Snackbar?.Add(Resources.Strings.CommandFailedToRunUnknown, Severity.Error);
            return;
        }

        foreach(var status in statuses)
        {
            ProcessStatus(status);
        }
    }

    private void ProcessStatus(ArduinoCommandStatus? status)
    {
        if(status is null)
        {
            
            Snackbar?.Add(Resources.Strings.CommandFailedToRunUnknown, Severity.Error);
            return;
        }
        var arduino = Arduinos.Where(x => x.Id == status.ArduinoId).FirstOrDefault();
        if (status.IsSuccess && !status.IsLongRunning)
        {
            Snackbar?.Add(string.Format(Resources.Strings.CommandSuccess, arduino?.Name), Severity.Success);
            return;
        }

        if (status.IsLongRunning)
        {
            Snackbar?.Add(string.Format(Resources.Strings.CommandSentButLongRunning, arduino?.Name));
            return;
        }
    }
}
