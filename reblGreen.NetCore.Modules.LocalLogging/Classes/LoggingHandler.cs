using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using reblGreen.Logging;
using reblGreen.Serialization;
using reblGreen.NetCore.Modules.Events;

namespace reblGreen.NetCore.Modules.LocalLogging.Classes
{
    [Serializable]
    internal class LoggingHandler
    {
        Module Module;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="logFileSize">The size of a log file in megabytes before rotating the log file.</param>
        /// <param name="logRotationFileCount">The number of log rotation files to keep.</param>
        internal LoggingHandler(Module module, ushort logFileSize, ushort logRotationFileCount)
        {
            Module = module;
            Log.AutoDebug = false;
            Log.AddLogger(new ConsoleLogger());
            Log.AddLogger(new ErrorFileLogger(Module.WorkingDirectory.LocalPath, logFileSize, logRotationFileCount));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        internal void LogEvent(LoggingEvent @event)
        {
            if (@event.Input != null)
            {
                switch (@event.Input.Severity)
                {
                    case LoggingEvent.Severity.Analytics:
                        Log.Analytic(@event.Input.Arguments.ToArray());
                        break;
                    case LoggingEvent.Severity.Debug:
                        Log.Information(@event.Input.Arguments.ToArray());
                        break;
                    case LoggingEvent.Severity.Error:
                        Log.Error(@event.Input.Arguments.ToArray());
                        break;
                    case LoggingEvent.Severity.Warning:
                        Log.Information(@event.Input.Arguments.ToArray());
                        break;
                }
            }
        }
    }
}
