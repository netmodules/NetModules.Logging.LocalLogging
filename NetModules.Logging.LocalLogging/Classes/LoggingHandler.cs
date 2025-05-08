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
        ConsoleLogger ConsoleLogger;
        FileLogger FileLogger;

        string LastLine;

        /// <summary>
        /// 
        /// </summary>
        internal LoggingHandler(Module module, LoggingEvent.Severity consoleLogLevel, LoggingEvent.Severity fileLogLevel)
        {
            Module = module;
            ConsoleLogger = new ConsoleLogger(consoleLogLevel);
            Loggers.AddLogger(ConsoleLogger);

            FileLogger = new FileLogger(Module, fileLogLevel);
            Loggers.AddLogger(FileLogger);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void LogEvent(LoggingEvent e)
        {
            if (e.Input.Arguments != null && e.Input.Arguments.Count > 0)
            {
                var printable = LoggingHelpers.GetPrintableArgs(e.Input.Arguments.ToArray());
                
                LastLine = $"{LoggingHelpers.GetDateString()}:{e.Input.Severity.ToString().ToUpperInvariant()} {string.Join("\n>", printable)}";

                Loggers.Log<LoggingEvent.Severity>(e.Input.Severity, printable);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void LastEvent(LastLineEvent e)
        {
            e.Output = new ReadLoggingFileEventOutput
            {
                Log = LastLine
            };

            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void SetLevelEvent(SetLoggingLevelEvent e)
        {
            FileLogger.SetMaxLoggingLevel(e.Input.Severity);
            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void ReadEvent(ReadLoggingFileEvent e)
        {
            e.Output = new ReadLoggingFileEventOutput
            {
                Log = FileLogger.Read(e.Input.Lines, e.Input.SkipLines, e.Input.ReadMode)
            };
            e.Handled= true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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
