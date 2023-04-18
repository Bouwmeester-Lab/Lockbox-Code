using LockboxControl.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Models
{
    public class Command : IEntity
    {
        //public Command(string name, string description, string commandLetter)
        //{
        //    Name = name;
        //    Description = description;
        //    CommandLetter = commandLetter;
        //}

        //public Command () { }
        public required long Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
       
        public required string CommandLetter { get; set; }
    }
}
