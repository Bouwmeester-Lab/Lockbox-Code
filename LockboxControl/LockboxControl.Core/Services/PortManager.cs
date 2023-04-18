using LockboxControl.Core.Models;
using Microsoft.Extensions.Options;
using System.IO.Ports;

namespace LockboxControl.Core.Services;


public class PortManager : IDisposable
{
    private readonly Dictionary<string, SerialPort> serialPorts = new Dictionary<string, SerialPort>();
    private readonly PortConfiguration portConfiguration;


    public PortManager(IOptions<PortConfiguration>? options = null)
    {
        portConfiguration = options?.Value ?? new PortConfiguration();
    }

    public void PickPorts(string[] portNames)
    {
        foreach (var portName in portNames)
        {
            if (!serialPorts.ContainsKey(portName))
            {
                serialPorts.Add(portName, new SerialPort(portName, portConfiguration.BaudRate));
            }
        }
    }

    public void SendCommand(Command command)
    {
        foreach (var (_, port) in serialPorts)
        {
            SendCommandToSinglePort(command, port);
        }
    }

    private void SendCommandToSinglePort(Command command, SerialPort serialPort)
    {
        if (serialPort.IsOpen)
        {
            // Send the command only if it's open
            serialPort.Write(command.CommandLetter);
        }
    }

    public void Open()
    {
        foreach(var (_, port) in serialPorts)
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
        }
    }

    public void Close()
    {
        foreach(var (_, port) in serialPorts)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }
    }

    public static string[] ListPorts()
    {
        return SerialPort.GetPortNames();
    }

    public void Dispose()
    {
        foreach (var (_, port) in serialPorts)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
            port.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}