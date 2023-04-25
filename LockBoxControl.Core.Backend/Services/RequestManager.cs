using LockBoxControl.Core.Models.SerialDTO;
using LockBoxControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LockBoxControl.Core.Contracts;
using LockBoxControl.Core.Models;

namespace LockBoxControl.Core.Backend.Services
{
    public class RequestManager
    {
        private readonly IGenericQueryableRepositoryService<Request, Guid> requestRepositoryService;
        public RequestManager(IGenericQueryableRepositoryService<Request, Guid> requestRepositoryService)
        {
            this.requestRepositoryService = requestRepositoryService;
        }

        public async Task<Request> CreateRequestAsync(Arduino arduino, Command command, CancellationToken cancellationToken = default)
        {
            var request = new Request
            {
                ArduinoId = arduino.Id,
                CommandId = command.Id,
                Id = Guid.NewGuid(),
                RequestDateTime = DateTime.UtcNow,
                IsCompleted = false,
                IsSuccess = false,
            };

            await requestRepositoryService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
            return request;
        }

        public async Task ProcessRequestAsync(SerialCommandStatus status, CancellationToken cancellationToken = default)
        {
            var request = await requestRepositoryService.GetAsync(status.RequestId, cancellationToken).ConfigureAwait(false);
            if (request == null) { return; }

            request.IsSuccess = status.IsOk;
            request.IsCompleted = true;
            request.CompletedDateTime = DateTime.UtcNow;
            await requestRepositoryService.UpdateAsync(request, cancellationToken).ConfigureAwait(false);
            //await requestRepositoryService.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
