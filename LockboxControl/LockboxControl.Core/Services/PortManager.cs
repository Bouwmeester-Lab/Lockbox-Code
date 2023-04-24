using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using LockboxControl.Core.Models.ApiDTO;
using LockboxControl.Core.Models.SerialDTO;
using Microsoft.Extensions.Options;
using System.IO.Ports;
using System.Text.Json;

namespace LockboxControl.Core.Services;


public class PortManager
{
    //private readonly Dictionary<string, SerialPort> serialPorts = new Dictionary<string, SerialPort>();

    private readonly PortConfiguration portConfiguration;
    private readonly IQueryableRepositoryService<Arduino> arduinoService;
    private readonly RequestManager requestManager;

    public PortManager(IQueryableRepositoryService<Arduino> arduinoService, RequestManager requestManager,  IOptions<PortConfiguration>? options = null)
    {
        portConfiguration = options?.Value ?? new PortConfiguration();
        this.arduinoService = arduinoService;
        this.requestManager = requestManager;
    }

    //public void PickPorts(string[] portNames)
    //{
    //    foreach (var portName in portNames)
    //    {
    //        if (!serialPorts.ContainsKey(portName))
    //        {
    //            serialPorts.Add(portName, new SerialPort(portName, portConfiguration.BaudRate));
    //        }
    //    }
    //}

    public async Task<List<ArduinoCommandStatus>> SendCommandAsync(Command command, CancellationToken cancellationToken = default)
    {
        var arduinos = await arduinoService.GetAllAsync(cancellationToken);
        var statuses = new List<ArduinoCommandStatus>();
        foreach (var arduino in arduinos)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            if (arduino.IsEnabled)
            {
                var status = await SendCommandToSinglePortAsync(command, arduino, cancellationToken).ConfigureAwait(false);

                statuses.Add(ArduinoCommandStatus.FromSerialCommandStatus(status, arduino.Id));
            }            
        }
        return statuses;
    }

    public async Task<ArduinoCommandStatus> SendCommandAsync(long arduinoId, Command command, CancellationToken cancellationToken = default)
    {
        var arduino = await arduinoService.GetAsync(arduinoId, cancellationToken) ?? throw new ArgumentOutOfRangeException(nameof(arduinoId));

        if(cancellationToken.IsCancellationRequested) 
        {
            throw new TaskCanceledException();
        }
        var status = await SendCommandToSinglePortAsync(command, arduino, cancellationToken).ConfigureAwait(false);
        return ArduinoCommandStatus.FromSerialCommandStatus(status, arduino.Id);
    }

    private async Task<SerialCommandStatus?> SendCommandToSinglePortAsync(Command command, Arduino arduino, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(arduino.PortName);

        // create request
        var request = await requestManager.CreateRequestAsync(arduino, command, cancellationToken).ConfigureAwait(false);

        using var serialPort = new SerialPort(arduino.PortName, portConfiguration.BaudRate);

        if (!serialPort.IsOpen)
        {
            serialPort.Open();
        }

        // Send the command only if it's open
        var commandJson = JsonSerializer.Serialize(new SerialCommand(command.CommandLetter, request.Id), options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        serialPort.WriteLine(commandJson);

        // Read the response (status)

        var json = serialPort.ReadLine();
        var status = JsonSerializer.Deserialize<SerialCommandStatus>(json);

        if(status != null && !status.IsLongRunning)
        {
            await requestManager.ProcessRequestAsync(status, cancellationToken).ConfigureAwait(false); // saves the request as completed. It logs the result.
        }

        return status;
    }


    public static string[] ListPorts()
    {
        return SerialPort.GetPortNames();
    }

}