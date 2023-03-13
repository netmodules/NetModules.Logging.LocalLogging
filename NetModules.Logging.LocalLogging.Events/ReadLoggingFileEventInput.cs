using NetModules.Interfaces;
using System;
using System.ComponentModel.Design;

namespace NetModules.Logging.LocalLogging.Events
{
    public struct ReadLoggingFileEventInput : IEventInput
    {
        public ushort Lines { get; set; }
    }
}
