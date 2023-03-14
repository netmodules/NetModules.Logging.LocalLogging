using NetModules;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using NetTools.Logging;
using NetTools.Serialization;
using NetModules.Events;
using Modules.Logging.LocalLogging.Events;

namespace Modules.Logging.LocalLogging.Classes
{
    [Serializable]
    internal class LoggingHandler
    {
        Module Module;
        FileLogger FileLogger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        /// <param name="logFileSize">The size of a log file in megabytes before rotating the log file.</param>
        /// <param name="logRotationFileCount">The number of log rotation files to keep.</param>
        internal LoggingHandler(Module module, ushort logFileSize, ushort logRotationFileCount, LoggingEvent.Severity maxLoggingLevel)
        {
            Module = module;
            Log.AutoDebug = false;
            Log.AddLogger(new ConsoleLogger());

            FileLogger = new FileLogger(Module, logFileSize, logRotationFileCount, maxLoggingLevel);
            Log.AddLogger(FileLogger);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        internal void LogEvent(LoggingEvent @event)
        {
            if (@event.Input.Arguments != null && @event.Input.Arguments.Count > 0)
            {
                switch (@event.Input.Severity)
                {
                    case LoggingEvent.Severity.Information:
                        Log.Analytic(@event.Input.Arguments.ToArray());
                        break;
                    case LoggingEvent.Severity.Debug:
                        Log.Debug(@event.Input.Arguments.ToArray());
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        internal void ReadEvent(ReadLoggingFileEvent @event)
        {
            @event.Output = new ReadLoggingFileEventOutput
            {
                Log = FileLogger.ReadFile(@event.Input.Lines)
            };
            @event.Handled= true;
        }
    }
}
