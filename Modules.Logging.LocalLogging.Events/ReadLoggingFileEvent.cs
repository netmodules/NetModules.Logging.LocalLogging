using NetModules;
using NetModules.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Modules.Logging.LocalLogging.Events
{
    public class ReadLoggingFileEvent : IEvent<ReadLoggingFileEventInput, ReadLoggingFileEventOutput>
    {
        public ReadLoggingFileEventInput Input { get; set; }
        public ReadLoggingFileEventOutput Output { get; set; }

        public EventName Name => "Logging.LocalLogging.ReadLoggingFile";

        public Dictionary<string, object> Meta { get; set; }
        
        public bool Handled { get; set; }

        public IEventInput GetEventInput()
        {
            return Input;
        }

        public IEventOutput GetEventOutput()
        {
            return Output;
        }

        public void SetEventOutput(IEventOutput output)
        {
            if (output is ReadLoggingFileEventOutput logging)
            {
                Output = logging;
            }
        }
    }
}
