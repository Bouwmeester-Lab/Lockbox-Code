using LockboxControl.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LockboxControl.Core.Models
{
    public class ArduinoStatus : IEntity<Guid>
    {
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }

        public DateTime StatusDateTime { get; set; }
    }

    public enum Status
    {
        Locked = 0,
        Unlocked = 1,

        
        
    }
}
