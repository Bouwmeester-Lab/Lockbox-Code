using LockboxControl.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Models
{
    public class Arduino : IEntity
    {
        public required long Id { get; set; }
        [MaxLength(255)]
        public required string Name { get; set; }
        [MaxLength(500)]
        public required string Description { get; set; }
        [MaxLength(25)]
        public string? PortName { get; set; }
        public bool IsEnabled { get; set; } = false;
    }
}
