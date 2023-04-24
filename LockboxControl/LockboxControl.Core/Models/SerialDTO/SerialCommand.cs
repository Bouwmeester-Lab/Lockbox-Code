using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Models.SerialDTO
{
    public record SerialCommand(string CommandLetter, Guid RequestId)
    {
        
    }
}
