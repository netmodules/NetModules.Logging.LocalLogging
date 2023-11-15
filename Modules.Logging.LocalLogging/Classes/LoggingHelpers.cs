using System;
using System.Text;
using System.Collections.Generic;
using NetModules;
using NetModules.Interfaces;
using NetTools.Serialization;

namespace Modules.Logging.LocalLogging.Classes
{
    internal static class LoggingHelpers
    {
        /// <summary>
        /// Returns the logging arguments as a printable string.
        /// </summary>
        internal static string GetPrintableArgs(params object[] args)
        {
            if (args == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg == null || arg is string s && string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                if (arg is Exception ex)
                {
                    sb.Append(UnwrapExceptionMessage(ex));
                }
                else if (arg is IEvent @event)
                {
                    sb.Append(arg.ToJson().BeautifyJson());
                }
                else
                {
                    sb.Append(arg);
                }

                sb.Append('\n');
                if (i < args.Length - 1)
                {
                    sb.Append('>');
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Returns an unwrapped string containing the exception and any inner exceptions.
        /// </summary>
        internal static string UnwrapExceptionMessage(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            return $"{ex}, Inner exception: {UnwrapExceptionMessage(ex.InnerException)} ";
        }


        /// <summary>
        /// Returns a formatted utc date/time string.
        /// </summary>
        internal static string GetDateString()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss.FFF");
        }
    }
}
