using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Models.SerialDTO
{
    public class SerialCommandStatus
    {
        [JsonPropertyName("isOk")]
        public bool IsOk { get; set; }
        [JsonPropertyName("isLongRunning")]
        public bool IsLongRunning { get; set; } = false;
        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }
        [JsonPropertyName("requestId")]
        public Guid RequestId { get; set; }
        [JsonPropertyName("result")]
        public object? Result { get; set; }
    }
}
