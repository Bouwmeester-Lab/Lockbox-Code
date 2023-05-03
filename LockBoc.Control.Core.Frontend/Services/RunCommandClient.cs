using LockBoxControl.Core.Models;
using LockBoxControl.Core.Models.ApiDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Frontend.Services
{
    public class RunCommandClient
    {
        private readonly string path = "/api/Run"; 
        private readonly HttpClient httpClient;
        public RunCommandClient(HttpClient httpClient) 
        {
            this.httpClient = httpClient;
        }

        public async Task<List<ArduinoCommandStatus>?> RunCommandAsync(Command command)
        {
            
            var response = await httpClient.PostAsync($"{path}/{command.Id}", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ArduinoCommandStatus>>().ConfigureAwait(false);
            }
            return null;
        }

        public async Task<List<ArduinoCommandStatus>?> RunCommandAsync(Command command, long arduinoId)
        {
            var response = await httpClient.PostAsync($"{path}/{command.Id}?arduinoId={arduinoId}", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ArduinoCommandStatus>>().ConfigureAwait(false);
            }
            return null;
        }

        public async Task<List<ArduinoCommandStatus>> RunCommandAsync(Command command, long[] arduinoIds)
        {
            var resultsTasks = new List<Task<List<ArduinoCommandStatus>?>>();
            foreach (var id in arduinoIds)
            {
                resultsTasks.Add(RunCommandAsync(command, id));
            }
            var resultsArray = await Task.WhenAll(resultsTasks);
            
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return resultsArray.SelectMany(x => x).Where(x => x != null).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }
    }
}
