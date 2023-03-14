using NetModules.Interfaces;
using System;
using System.ComponentModel.Design;

namespace Modules.Logging.LocalLogging.Events
{
    public class ReadLoggingFileEventOutput : IEventOutput
    {
        public string Log { get; set; }
    }
}
