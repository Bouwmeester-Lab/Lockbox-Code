using LockBoxControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Frontend.Services
{
    public class ArduinoStatusesClient : CrudClient<ArduinoStatus, Guid>
    {
        public ArduinoStatusesClient(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override string Path => "/api/Statuses";
    }
}
