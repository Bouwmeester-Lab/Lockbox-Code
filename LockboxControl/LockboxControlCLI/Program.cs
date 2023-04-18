// See https://aka.ms/new-console-template for more information
using LockboxControl.Core.Services;
using LockboxControl.Core.Models;
//Console.WriteLine("Hello, World!");


using var portManager = new PortManager();

var portNames = PortManager.ListPorts();

Console.WriteLine($"There's {portNames.Length} available ports.");
foreach (var port in portNames)
{
    Console.WriteLine($"Adding {port} to the port manager");    
}
portManager.PickPorts(portNames);

// open the port connections
portManager.Open();

portManager.SendCommand(new Command()
{
    Id = 1,
    Name = "Scan",
    Description = "scans the multiple lockboxes",
    CommandLetter = "s"
});