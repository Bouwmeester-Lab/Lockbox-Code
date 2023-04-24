using LockboxControl.Core.Contracts;
using LockboxControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Services
{
    /// <summary>
    /// Class intended to manage the reporting of statuses by the different arduinos in the system.
    /// </summary>
    public class PingManager
    {
        private readonly IGenericQueryableRepositoryService<ArduinoStatus, Guid> arduinoStatusesRepositoryService;
        private readonly PortManager portManager;
        public PingManager(IGenericQueryableRepositoryService<ArduinoStatus, Guid> arduinoStatusesRepositoryService, PortManager portManager)
        {
            this.arduinoStatusesRepositoryService = arduinoStatusesRepositoryService;
            this.portManager = portManager;
        }

        public Task RegisterArduinoAsync(string macAddress, CancellationToken cancellationToken = default)
        {
            
        }
    }
}
