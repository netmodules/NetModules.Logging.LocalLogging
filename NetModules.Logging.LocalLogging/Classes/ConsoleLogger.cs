using System;
using NetTools.Logging;
using NetModules.Events;
using NetModules.Interfaces;

namespace NetModules.Logging.LocalLogging.Classes
{
    internal class ConsoleLogger : ILogger<LoggingEvent.Severity>
    {
        bool Enabled;
        bool HasConsole;
        LoggingEvent.Severity LoggingLevel;

        internal ConsoleLogger(IModule module, LoggingEvent.Severity logLevel)
        {
            Enabled = module.GetSetting("enableConsoleLogger", true);
            HasConsole = HasConsoleWindow();
            SetLoggingLevel(logLevel);
        }


        public void Log(params object[] args)
        {
            if (!Enabled || !HasConsole)
            {
                return;
            }

            SetConsoleColor(LoggingHelpers.GetLoggingColor(LoggingEvent.Severity.Trace));
            LogString(string.Join("\n>", args));
        }

        public void Log(LoggingEvent.Severity level, params object[] args)
        {
            if (!Enabled || !HasConsole || level < LoggingLevel)
            {
                return;
            }

            SetConsoleColor(LoggingHelpers.GetLoggingColor(level));
            LogString($"{LoggingHelpers.GetDateString(true)}:{level.ToString().ToUpperInvariant()} {string.Join("\n>", args)}");
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


        internal void SetLoggingLevel(LoggingEvent.Severity logLevel)
        {
            LoggingLevel = logLevel;
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
