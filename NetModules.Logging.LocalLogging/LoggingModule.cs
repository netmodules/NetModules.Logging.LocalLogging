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
                || e is LastLineEvent
                || e is ReadLoggingFileEvent
                || e is SearchLoggingFileEvent
                || e is SetLoggingLevelEvent)
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

            if (e is LastLineEvent last)
            {
                LoggingHandler.LastEvent(last);
                return;
            }

            if (e is ReadLoggingFileEvent read)
            {
                LoggingHandler.ReadEvent(read);
                return;
            }

            if (e is SearchLoggingFileEvent search)
            {
                LoggingHandler.SearchEvent(search);
                return;
            }

            if (e is SetLoggingLevelEvent set)
            {
                LoggingHandler.SetLevelEvent(set);
                return;
            }
        }

        public override void OnLoaded()
        {
            var logLevel = GetSetting("consoleLogLevel", string.Empty);
            var fileLogLevel = !string.IsNullOrEmpty(logLevel)
                && Enum.TryParse<LoggingEvent.Severity>(logLevel, true, out var ll)
                ? ll
                : LoggingEvent.Severity.Error;

            logLevel = GetSetting("fileLogLevel", string.Empty);
            
            var consoleLogLevel = !string.IsNullOrEmpty(logLevel)
                && Enum.TryParse<LoggingEvent.Severity>(logLevel, true, out ll)
                ? ll
                : LoggingEvent.Severity.Error;

            if (Host.Arguments != null)
            {
                for (var i = 0; i < Host.Arguments.Count; i++)
                {
                    var arg = Host.Arguments[i].ToLowerInvariant();
                    if ((arg == "-loglevel" || arg == "-log" || arg == "-l") && Host.Arguments.Count > i)
                    {
                        var value = Host.Arguments[i + 1].ToLowerInvariant();

                        switch (value.Trim('"'))
                        {
                            case "debug":
                                fileLogLevel = LoggingEvent.Severity.Debug;
                                break;
                            case "analytics":
                            case "information":
                            case "info":
                                fileLogLevel = LoggingEvent.Severity.Information;
                                break;
                            case "warning":
                                fileLogLevel = LoggingEvent.Severity.Warning;
                                break;
                            default:
                                fileLogLevel = LoggingEvent.Severity.Error;
                                break;
                        }
                        
                        break;
                    }
                }
            }

            LoggingHandler = new LoggingHandler(this, consoleLogLevel, fileLogLevel);
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
