using LockBoxControl.Core.Contracts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LockBoxControl.Core.Models;

public class ArduinoStatus : IEntity<Guid>
{
    public Guid Id { get; set; }

    [Column(TypeName = "nvarchar(24)")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Status Status { get; set; }

    public DateTime StatusDateTime { get; set; }

    public long ArduinoId { get; set; }
    public Arduino? Arduino { get; set; }
}

public enum Status
{
    Locked = 0,
    Unlocked = 1,



}
