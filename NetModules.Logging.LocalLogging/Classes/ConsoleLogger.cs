using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NetModules;
using NetTools.Logging;
using NetModules.Interfaces;
using NetTools.Serialization;
using NetModules.Events;
using System.Collections;

namespace Modules.Logging.LocalLogging.Classes
{
    internal class ConsoleLogger : ILogger<LoggingEvent.Severity>
    {
        bool HasConsole;
        LoggingEvent.Severity MaxLoggingLevel;

        internal ConsoleLogger(LoggingEvent.Severity maxLoggingLevel)
        {
            HasConsole = HasConsoleWindow();
            SetMaxLoggingLevel(maxLoggingLevel);
        }


        public void Log(params object[] args)
        {
            if (!HasConsole)
            {
                return;
            }

            LogString(string.Join("\n>", args));
        }

        public void Log(LoggingEvent.Severity level, params object[] args)
        {
            if (!HasConsole || level > MaxLoggingLevel)
            {
                return;
            }

            LogString($"{LoggingHelpers.GetDateString()}:{level.ToString().ToUpperInvariant()} {string.Join("\n>", args)}");
        }


        void LogString(string str)
        {
            if (HasConsole)
            {
                Console.Write(str);
                ResetConsoleColor();
                Console.WriteLine();
            }
        }


        internal void SetMaxLoggingLevel(LoggingEvent.Severity maxLoggingLevel)
        {
            MaxLoggingLevel = maxLoggingLevel;
        }

        internal void SetConsoleColor(ConsoleColor color)
        {
            if (HasConsole)
            {
                Console.ForegroundColor = color;
            }
        }

        internal void ResetConsoleColor()
        {
            if (HasConsole)
            {
                Console.ResetColor();
            }
        }

        bool HasConsoleWindow()
        {
            try
            {
                return Console.WindowHeight > -1;
            }
            catch
            {
                return false;
            }
        }
    }
}
