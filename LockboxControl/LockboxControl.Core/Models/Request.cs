﻿using LockBoxControl.Core.Contracts;

namespace LockBoxControl.Core.Models
{
    public class Request : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public long ArduinoId { get; set; }
        public Arduino? Arduino { get; set; }

        public long CommandId { get; set; }
        public Command? Command { get; set; }

        public DateTime RequestDateTime { get; set; }
        public DateTime CompletedDateTime { get; set; }

        public bool IsSuccess { get; set; }
        public bool IsCompleted { get; set; }
    }
}
