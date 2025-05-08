using System;
using System.Collections.Generic;
using NetTools.Serialization;
using NetModules.Interfaces;
using NetModules.Events;

namespace NetModules.Logging.LocalLogging.Classes
{
    internal static class LoggingHelpers
    {
        /// <summary>
        /// Returns the logging arguments as a printable string.
        /// </summary>
        internal static string[] GetPrintableArgs(object[] args)
        {
            if (args == null)
            {
                return new string[0];
            }

            var printable = new List<string>();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg == null || arg is string s && string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                if (arg is Exception ex)
                {
                    printable.Add(UnwrapException(ex));
                    continue;
                }
                
                if (arg is IEvent @event)
                {
                    printable.Add(arg.ToJson().BeautifyJson());
                    continue;
                }

                printable.Add(arg.ToString());
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
        /// Get a Console Foreground Color based on the logging severity.
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
