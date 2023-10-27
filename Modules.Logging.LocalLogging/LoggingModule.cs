using System;
using NetModules;
using NetModules.Events;
using NetModules.Interfaces;
using Modules.Logging.LocalLogging.Classes;
using Modules.Logging.LocalLogging.Events;

namespace Modules.Logging.LocalLogging
{
    /// <summary>
    /// A basic logging module. This module writes all LoggingEvent data to the console output when available and also
    /// writes error logs to a file in the Module.WorkingDirectory path using log rotation.
    /// </summary>
    [Serializable]
    [Module(
        LoadFirst = true, LoadPriority = short.MinValue + 1, HandlePriority = short.MaxValue,
        Description = "A basic logging module. This module writes all LoggingEvent data to the console output when "
        + "available and also writes error logs to a file in the Module.WorkingDirectory path using log rotation."
    )]
    public class LoggingModule : Module
    {
        LoggingHandler LoggingHandler;

        public override bool CanHandle(IEvent e)
        {
            if (e is LoggingEvent
                || e is ReadLoggingFileEvent)
            {
                return true;
            }

            return false;
        }

        public override void Handle(IEvent e)
        {
            if (e is LoggingEvent @event)
            {
                if (LoggingHandler != null)
                {
                    LoggingHandler.LogEvent(@event);
                }

                return;
            }

            if (e is ReadLoggingFileEvent read)
            {
                LoggingHandler.ReadEvent(read);
            }
        }

        public override void OnLoaded()
        {
            var logFileSize = (ushort)GetSetting("logFileSize", 100);
            var logRotationFileCount = (ushort)GetSetting("logRotationFileCount", 10);
            var maxLogLevel = LoggingEvent.Severity.Debug;

            if (Host.Arguments != null)
            {
                for (var i = 0; i < Host.Arguments.Count; i++)
                {
                    var arg = Host.Arguments[i].ToLowerInvariant();
                    if (arg == "-loglevel" || arg == "-log" || arg == "-l")
                    {
                        if (Host.Arguments.Count > i)
                        {
                            var value = Host.Arguments[i + 1].ToLowerInvariant();

                            switch (value)
                            {
                                case "warning":
                                    maxLogLevel = LoggingEvent.Severity.Warning;
                                    break;
                                case "debug":
                                    maxLogLevel = LoggingEvent.Severity.Debug;
                                    break;
                                case "analytics":
                                case "information":
                                case "info":
                                    maxLogLevel = LoggingEvent.Severity.Information;
                                    break;
                                default:
                                    maxLogLevel = LoggingEvent.Severity.Error;
                                    break;
                            }
                        }

                        break;
                    }
                }
            }
            LoggingHandler = new LoggingHandler(this, logFileSize, logRotationFileCount, maxLogLevel);
            base.OnLoaded();
        }


        public override void OnLoading()
        {
            base.OnLoaded();
        }


        public override void OnUnloading()
        {
            base.OnUnloading();
        }
    }
}
