using LockBoxControl.Core.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace LockBoxControl.Core.Models;

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


    [NotMapped]
    public static Command MacAddressCommand => new Command
    {
        CommandLetter = "m",
        Description = "Gets the mac address of the arduino.",
        Name = "Mac Address",
        Id = 0,
    };

    public override string ToString()
    {
        return Name;
    }
}
