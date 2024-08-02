using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NetModules;
using NetTools.Logging;
using NetModules.Interfaces;
using NetTools.Serialization;

namespace Modules.Logging.LocalLogging.Classes
{
    internal class ConsoleLogger : ILogger
    {
        bool HasConsole;
        ConsoleColor DefaultColor;

        internal ConsoleLogger()
        {
            HasConsole = HasConsoleWindow();

            if (HasConsole)
            {
                DefaultColor = Console.ForegroundColor;
            }
        }

        public void Analytic(params object[] args)
        {
            var logArgs = LoggingHelpers.GetPrintableArgs(args);

            if (!string.IsNullOrWhiteSpace(logArgs))
            {
                LogDate(ConsoleColor.Cyan, "information");
                LogString(logArgs);
            }
        }

        public void Debug(params object[] args)
        {
            var logArgs = LoggingHelpers.GetPrintableArgs(args);

            if (!string.IsNullOrWhiteSpace(logArgs))
            {
                LogDate(ConsoleColor.Cyan, "debug");
                LogString(logArgs);
            }
        }

        public void Error(params object[] args)
        {
            var logArgs = LoggingHelpers.GetPrintableArgs(args);

            if (!string.IsNullOrWhiteSpace(logArgs))
            {
                LogDate(ConsoleColor.Cyan, "error");
                LogString(logArgs);
            }
        }

        public void Information(params object[] args)
        {
            var logArgs = LoggingHelpers.GetPrintableArgs(args);

            if (!string.IsNullOrWhiteSpace(logArgs))
            {
                LogDate(ConsoleColor.Cyan, "warning");
                LogString(logArgs);
            }
        }

        
        void LogDate(ConsoleColor color, string logType)
        {
            if (HasConsole)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{LoggingHelpers.GetDateString()}> {logType.ToUpperInvariant()}:");
                Console.ForegroundColor = DefaultColor;
            }
        }

        void LogString(string str)
        {
            if (HasConsole)
            {
                Console.WriteLine(str);
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
