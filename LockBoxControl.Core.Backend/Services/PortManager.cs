using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models.ApiDTO;
using LockBoxControl.Core.Models.SerialDTO;
using LockBoxControl.Core.Models;
using Microsoft.Extensions.Options;
using System.IO.Ports;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LockBoxControl.Core.Backend.Services;


public class PortManager
{
    //private readonly Dictionary<string, SerialPort> serialPorts = new Dictionary<string, SerialPort>();

    private readonly PortConfiguration portConfiguration;
    private readonly IQueryableRepositoryService<Arduino> arduinoService;
    private readonly RequestManager requestManager;
    private readonly IQueryableRepositoryService<Command> commandService;
    private readonly IServiceProvider serviceProvider;

    public PortManager(IQueryableRepositoryService<Arduino> arduinoService, IQueryableRepositoryService<Command> commandService, RequestManager requestManager, IServiceProvider serviceProvider, IOptions<PortConfiguration>? options = null)
    {
        portConfiguration = options?.Value ?? new PortConfiguration();
        this.arduinoService = arduinoService;
        this.requestManager = requestManager;
        this.commandService = commandService;
        this.serviceProvider = serviceProvider;
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

        if (cancellationToken.IsCancellationRequested)
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
            try
            {
                serialPort.Open();
            }
            catch(FileNotFoundException ex)
            {

                 var error = new SerialCommandStatus
                {
                    ErrorMessage = $"Could not open serial port {arduino.PortName} - {ex.Message}",
                    IsOk = false,
                    RequestId = request.Id,
                    IsLongRunning = false,
                };
                await requestManager.ProcessRequestAsync(error, cancellationToken).ConfigureAwait(false); // saves the request as completed. It logs the result.
                return error;
            }
            catch(UnauthorizedAccessException ex)
            {
                var error = new SerialCommandStatus
                {
                    ErrorMessage = $"Serial port {arduino.PortName} is in use!!\n {ex.Message}",
                    IsOk = false,
                    RequestId = request.Id,
                    IsLongRunning = false,
                };
                await requestManager.ProcessRequestAsync(error, cancellationToken).ConfigureAwait(false); // saves the request as completed. It logs the result.
                return error;
            }
            
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

        if (status != null && !status.IsLongRunning)
        {
            await requestManager.ProcessRequestAsync(status, cancellationToken).ConfigureAwait(false); // saves the request as completed. It logs the result.
        }

        return status;
    }

    public async Task<string?> GetMacAddressAsync(Arduino arduino, CancellationToken cancellationToken = default)
    {
        // this must be able to be called from multiple threads
        using var serviceScope = serviceProvider.CreateScope();

        var macCommand = await commandService.QueryAll(serviceScope).Where(x => x.CommandLetter == Command.MacAddressCommand.CommandLetter).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        if (macCommand != null)
        {
            var status = await SendCommandToSinglePortAsync(macCommand, arduino, cancellationToken).ConfigureAwait(false);

           if(status != null && !status.IsLongRunning && status.IsOk)
           {
                // the result is the mac address
                var macAddress = status.Result?.ToString() ?? throw new InvalidOperationException("The result cannot be empty when obtaining the mac address.");
                return macAddress;
           }
        }
        return default;
    }


    public static string[] ListPorts()
    {
        return SerialPort.GetPortNames();
    }

}