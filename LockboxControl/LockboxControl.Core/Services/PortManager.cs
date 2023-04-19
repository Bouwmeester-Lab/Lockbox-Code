using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using Microsoft.Extensions.Options;
using System.IO.Ports;

namespace LockboxControl.Core.Services;


public class PortManager
{
    //private readonly Dictionary<string, SerialPort> serialPorts = new Dictionary<string, SerialPort>();

    private readonly PortConfiguration portConfiguration;
    private readonly IQueryableRepositoryService<Arduino> arduinoService;

    public PortManager(IQueryableRepositoryService<Arduino> arduinoService, IOptions<PortConfiguration>? options = null)
    {
        portConfiguration = options?.Value ?? new PortConfiguration();
        this.arduinoService = arduinoService;
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

    public async Task SendCommandAsync(Command command, CancellationToken cancellationToken = default)
    {
        var arduinos = await arduinoService.GetAllAsync(cancellationToken);

        foreach (var arduino in arduinos)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (arduino.IsEnabled)
            {
                SendCommandToSinglePort(command, arduino);
            }            
        }
    }

    private void SendCommandToSinglePort(Command command, Arduino arduino)
    {
        ArgumentException.ThrowIfNullOrEmpty(arduino.PortName);

        using var serialPort = new SerialPort(arduino.PortName, portConfiguration.BaudRate);

        if (!serialPort.IsOpen)
        {
            serialPort.Open();
        }

        // Send the command only if it's open
        serialPort.Write(command.CommandLetter);

    }

    

    public static string[] ListPorts()
    {
        return SerialPort.GetPortNames();
    }

}