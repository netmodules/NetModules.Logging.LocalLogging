using System;
using System.Text;
using System.Collections.Generic;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization;
using NetModules.Events;

namespace Modules.Logging.LocalLogging.Classes
{
    internal static class LoggingHelpers
    {
        /// <summary>
        /// Returns the logging arguments as a printable string.
        /// </summary>
        internal static string[] GetPrintableArgs(object[] args, out LoggingEvent.Severity logLevel)
        {
            logLevel = LoggingEvent.Severity.Debug;
            
            if (args == null)
            {
                return new string[0];
            }

            var printable = new List<string>();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg is LoggingEvent.Severity severity)
                {
                    logLevel = severity;
                    printable.Add(severity.ToString().ToUpperInvariant());
                    continue;
                }

                if (arg == null || arg is string s && string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                if (arg is Exception ex)
                {
                    printable.Add(UnwrapException(ex));
                }
                else if (arg is IEvent @event)
                {
                    printable.Add(arg.ToJson().BeautifyJson());
                }
                else
                {
                    printable.Add(arg.ToString());
                }
            }

            return printable.ToArray();
        }


        /// <summary>
        /// Returns an unwrapped string containing the exception and any inner exceptions.
        /// </summary>
        internal static string UnwrapException(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            return $"{ex.ToString().Replace('\n', '>')}{(ex.InnerException != null ? $", Inner exception: {UnwrapException(ex.InnerException)}" : string.Empty)}";
        }


        /// <summary>
        /// Returns a formatted utc date/time string.
        /// </summary>
        internal static string GetDateString(bool includeMicrosecond = false)
        {
            return DateTime.UtcNow.ToString("[yyyy/MM/dd HH:mm:ss" + (includeMicrosecond ? ".fff" : string.Empty) + "]");
        }


        /// <summary>
        /// 
        /// </summary>
        internal static ConsoleColor GetLoggingColor(LoggingEvent.Severity severity)
        {
            return severity switch
            {
                LoggingEvent.Severity.Trace => ConsoleColor.DarkGray,
                LoggingEvent.Severity.Debug => ConsoleColor.White,
                LoggingEvent.Severity.Information
                    or LoggingEvent.Severity.Notice => ConsoleColor.Cyan,
                LoggingEvent.Severity.Warning => ConsoleColor.Yellow,
                LoggingEvent.Severity.Error => ConsoleColor.Red,
                _ => ConsoleColor.Magenta
            };
        }
    }
}
