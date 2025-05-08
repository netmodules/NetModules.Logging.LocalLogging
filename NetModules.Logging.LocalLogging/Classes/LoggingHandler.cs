using System;
using NetTools.Logging;
using NetModules.Events;
using NetModules.Logging.LocalLogging.Events;

namespace NetModules.Logging.LocalLogging.Classes
{
    /// <summary>
    /// A handler class that handles all logging events and routing to registered loggers. We use a class here to
    /// reduce logic bloat in <see cref="LoggingModule"/>, making <see cref="LoggingModule"/> easier to read and maintain.
    /// </summary>
    [Serializable]
    internal class LoggingHandler
    {
        Module Module;
        ConsoleLogger ConsoleLogger;
        FileLogger FileLogger;

        string LastLine;

        /// <summary>
        /// A handler class that handles all logging events and routing to registered loggers. We use a class here to
        /// reduce logic bloat in <see cref="LoggingModule"/>, making <see cref="LoggingModule"/> easier to read and maintain.
        /// </summary>
        internal LoggingHandler(Module module, LoggingEvent.Severity consoleLogLevel, LoggingEvent.Severity fileLogLevel)
        {
            Module = module;
            ConsoleLogger = new ConsoleLogger(Module, consoleLogLevel);
            FileLogger = new FileLogger(Module, fileLogLevel);

            Loggers.AddLogger(ConsoleLogger);
            Loggers.AddLogger(FileLogger);
        }


        /// <summary>
        /// This method handles any <see cref="NetModules.Events.LoggingEvent"/> that can be raised by any
        /// loaded <see cref="NetModules.Interfaces.IModule"/> using a directly instantiated LoggingEvent
        /// or the <see cref="NetModules.Interfaces.IModule.Log(LoggingEvent.Severity, object[])"/> wrapper method.
        /// Any incoming <see cref="NetModules.Events.LoggingEvent"/> will be logged to the console and/or file.
        /// </summary>
        internal void LogEvent(LoggingEvent e)
        {
            if (e.Input.Arguments != null && e.Input.Arguments.Count > 0)
            {
                var printable = LoggingHelpers.GetPrintableArgs(e.Input.Arguments.ToArray());
                
                LastLine = $"{LoggingHelpers.GetDateString(true)}:{e.Input.Severity.ToString().ToUpperInvariant()} {string.Join("\n>", printable)}";

                Loggers.Log<LoggingEvent.Severity>(e.Input.Severity, printable);
            }
        }


        /// <summary>
        /// This method handles an incoming <see cref="NetModules.Logging.LocalLogging.Events.LastLineEvent"/> event
        /// that is received by <see cref="NetModules.Logging.LocalLogging.LoggingModule"/> module and is used get the last
        /// log message.
        /// </summary>
        internal void LastEvent(LastLineEvent e)
        {
            e.Output = new ReadLoggingFileEventOutput
            {
                Log = LastLine
            };

            e.Handled = true;
        }


        /// <summary>
        /// This method handles an incoming <see cref="NetModules.Logging.LocalLogging.Events.SetLoggingLevelEvent"/> event
        /// that is received by <see cref="NetModules.Logging.LocalLogging.LoggingModule"/> module and is used to dynamically
        /// set the log level for any registered loggers.
        /// </summary>
        internal void SetLogLevelEvent(SetLoggingLevelEvent e)
        {
            if (e.Input.Logger == Events.Enums.Logger.All || e.Input.Logger == Events.Enums.Logger.Console)
            {
                ConsoleLogger.SetLoggingLevel(e.Input.Severity);
            }

            if (e.Input.Logger == Events.Enums.Logger.All || e.Input.Logger == Events.Enums.Logger.File)
            {
                FileLogger.SetLoggingLevel(e.Input.Severity);
            }

            e.Handled = true;
        }


        /// <summary>
        /// This method handles an incoming <see cref="NetModules.Logging.LocalLogging.Events.ReadLoggingFileEvent"/> event
        /// that is received by <see cref="NetModules.Logging.LocalLogging.LoggingModule"/> module and is used read logged
        /// data from the log file (if available).
        /// </summary>
        internal void ReadEvent(ReadLoggingFileEvent e)
        {
            e.Output = new ReadLoggingFileEventOutput
            {
                Log = FileLogger.Read(e.Input.Lines, e.Input.SkipLines, e.Input.ReadMode)
            };
            e.Handled= true;
        }


        /// <summary>
        /// This method handles an incoming <see cref="NetModules.Logging.LocalLogging.Events.SearchLoggingFileEvent"/> event
        /// that is received by <see cref="NetModules.Logging.LocalLogging.LoggingModule"/> module and is used search logged
        /// data from the log file (if available). 
        /// </summary>
        internal void SearchEvent(SearchLoggingFileEvent e)
        {
            e.Output = new ReadLoggingFileEventOutput
            {
                Log = FileLogger.Search(e.Input.Query, e.Input.MaxLines)
            };
            e.Handled = true;
        }
    }
}
