using System;
using NetModules.Interfaces;

namespace Modules.Logging.LocalLogging.Events
{
    public struct ReadLoggingFileEventInput : IEventInput
    {
        public ushort Lines { get; set; }
    }
}
