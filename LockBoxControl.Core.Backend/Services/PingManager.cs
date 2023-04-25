using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;
using LockBoxControl.Core.Models.ApiDTO;
using Microsoft.EntityFrameworkCore;

namespace LockBoxControl.Core.Backend.Services;

/// <summary>
/// Class intended to manage the reporting of statuses by the different arduinos in the system.
/// </summary>
public class PingManager
{
    private readonly IGenericQueryableRepositoryService<ArduinoStatus, Guid> arduinoStatusesRepositoryService;
    private readonly PortManager portManager;
    private readonly IQueryableRepositoryService<Arduino> arduinoService;
    
    public PingManager(IGenericQueryableRepositoryService<ArduinoStatus, Guid> arduinoStatusesRepositoryService, PortManager portManager, IQueryableRepositoryService<Arduino> arduinoService)
    {
        this.arduinoStatusesRepositoryService = arduinoStatusesRepositoryService;
        this.portManager = portManager;
        this.arduinoService = arduinoService;
    }

    public async Task RegisterArduinoAsync(string macAddress, CancellationToken cancellationToken = default)
    {
        var arduinos = await arduinoService.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var results = new Dictionary<Arduino, Task<string?>>();
        
        foreach(var arduino in arduinos)
        {
            results.Add(arduino, portManager.GetMacAddressAsync(arduino, cancellationToken));
        }

        await Task.WhenAll(results.Values).ConfigureAwait(false);

        // we check which arduino has which this mac

        foreach(var (arduino, macTask) in results)
        {
            var address = await macTask;
            if(address != null && string.Equals(address, macAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                arduino.MacAddress = macAddress.ToLowerInvariant();
                await arduinoService.UpdateAsync(arduino, cancellationToken).ConfigureAwait(false);
                break; // we have found our match move on.
            }
        }
    }

    public async Task<ArduinoStatus?> UpdateStatusAsync(ArduinoStatusDTO arduinoStatus, CancellationToken cancellationToken = default)
    {
        var arduino = await arduinoService.QueryAll().Where(x => x.MacAddress != null && x.MacAddress.Equals(arduinoStatus.macAddress.ToLower())).FirstOrDefaultAsync(cancellationToken);
        
        if(arduino != null)
        {
            return await arduinoStatusesRepositoryService.CreateAsync(new ArduinoStatus
            {
                Id = Guid.NewGuid(),
                Status = arduinoStatus.Status,
                StatusDateTime = DateTime.UtcNow,
                ArduinoId = arduino.Id,
            }, cancellationToken);
        }
        return null;
    }
}
