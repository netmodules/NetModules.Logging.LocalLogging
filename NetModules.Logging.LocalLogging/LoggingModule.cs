using System;
using NetModules.Interfaces;
using NetModules.Events;
using NetModules.Logging.LocalLogging.Classes;
using NetModules.Logging.LocalLogging.Events;
using static NetModules.Events.LoggingEvent;

namespace NetModules.Logging.LocalLogging
{
    /// <summary>
    /// A basic local logging module. This module writes all LoggingEvent data to the console output when available,
    /// and can also write logs to a file in a location relative to the Module.WorkingDirectory path using log rotation.
    /// </summary>
    [Serializable]
    [Module(
        LoadFirst = true, LoadPriority = short.MinValue + 1, HandlePriority = short.MaxValue,
        Description = "A basic local logging module. This module writes all LoggingEvent data to the console output when available, "
        + "and can also write logs to a file in a location relative to the Module.WorkingDirectory path using log rotation."
    )]
    public class LoggingModule : Module
    {
        LoggingHandler LoggingHandler;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
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
                LoggingHandler.SetLogLevelEvent(set);
                return;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnLoading()
        {
            var logLevel = GetSetting("consoleLogLevel", "debug");
            var consoleLogLevel = !string.IsNullOrEmpty(logLevel)
                && Enum.TryParse<LoggingEvent.Severity>(logLevel, true, out var ll)
                ? ll
                : LoggingEvent.Severity.Error;

            logLevel = GetSetting("fileLogLevel", "error");
            
            var fileLogLevel = !string.IsNullOrEmpty(logLevel)
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
                        var value = Host.Arguments[i + 1].ToLowerInvariant().Trim('"');

                        switch (value)
                        {
                            case "trace":
                                fileLogLevel = Severity.Trace;
                                break;
                            case "debug":
                                fileLogLevel = Severity.Debug;
                                break;
                            case "information":
                            case "info":
                                fileLogLevel = Severity.Information;
                                break;
                            case "warn":
                            case "warning":
                                fileLogLevel = Severity.Warning;
                                break;
                            case "err":
                            case "error":
                                fileLogLevel = Severity.Error;
                                break;
                            default:
                                break;
                        }
                        
                        break;
                    }
                }
            }

            LoggingHandler = new LoggingHandler(this, consoleLogLevel, fileLogLevel);
            base.OnLoading();
        }
    }
}
