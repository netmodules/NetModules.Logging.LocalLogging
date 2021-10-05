using reblGreen;
using reblGreen.NetCore.Modules;
using System;
using System.Text;
using System.Collections.Generic;
using reblGreen.Logging;
using System.Linq;
using reblGreen.NetCore.Modules.Interfaces;
using reblGreen.Serialization;

namespace Modules.LocalLogging.Classes
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
            LogDate(ConsoleColor.Green, "analytic");
            LogArgs(args);
        }

        public void Debug(params object[] args)
        {
            LogDate(DefaultColor, "debug");
            LogArgs(args);
        }

        public void Error(params object[] args)
        {
            LogDate(ConsoleColor.Red, "error");
            LogArgs(args);
        }

        public void Information(params object[] args)
        {
            LogDate(ConsoleColor.Cyan, "information");
            LogArgs(args);
        }

        
        void LogDate(ConsoleColor color, string logType)
        {
            if (HasConsole)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{LoggingHelpers.GetDateString()} {logType.ToUpperInvariant()}:");
                Console.ForegroundColor = DefaultColor;
            }
        }

        void LogArgs(params object[] args)
        {
            if (HasConsole)
            {
                Console.WriteLine(LoggingHelpers.GetPrintableArgs(args));
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
