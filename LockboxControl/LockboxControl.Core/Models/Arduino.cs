using LockBoxControl.Core.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockBoxControl.Core.Models
{
    public class Arduino : IEntity
    {
        public required long Id { get; set; }
        [MaxLength(255)]
        [MinLength(2)]
        [Required]
        public required string Name { get; set; }
        [MaxLength(500)]
        //[MinLength(3)]
        [StringLength(500, ErrorMessage = "Description must be at least 3 characters long and 500 max.", MinimumLength = 3)]
        [Required]
        public required string Description { get; set; }
        [MaxLength(25)]
        public string? PortName { get; set; }
        public bool IsEnabled { get; set; } = false;

        [MaxLength(17)]
        public string? MacAddress { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
