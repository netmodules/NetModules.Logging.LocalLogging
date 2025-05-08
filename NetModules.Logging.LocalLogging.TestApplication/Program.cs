using System;
using System.Threading;
using NetModules.Events;
using NetModules.Logging.LocalLogging.Events;

namespace NetModules.Logging.LocalLogging.TestApplication
{
    class Program
    {
        static EventWaitHandle BlockingHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static void Main(string[] args)
        {
            ModuleHost host = new BasicModuleHost();
            host.Arguments.Add("-loglevel");
            host.Arguments.Add("error");

            host.Modules.LoadModules();

            host.Handle(new SetLoggingLevelEvent()
            {
                Input = new SetLoggingLevelEventInput()
                {
                    Logger = Events.Enums.Logger.All,
                    Severity = LoggingEvent.Severity.Trace
                }
            });

            host.Log(LoggingEvent.Severity.Trace, "Hello trace!");
            host.Log(LoggingEvent.Severity.Debug, "Hello debug!");
            host.Log(LoggingEvent.Severity.Information, "Hello information!");
            host.Log(LoggingEvent.Severity.Notice, "Hello notice!");
            host.Log(LoggingEvent.Severity.Warning, "Hello warning!");
            host.Log(LoggingEvent.Severity.Error, "Hello error!");
            host.Log(LoggingEvent.Severity.Critical, "Hello critical!");
            host.Log(LoggingEvent.Severity.Alert, "Hello alert!");
            host.Log(LoggingEvent.Severity.Emergency, "Hello emergency!");

            var myModule = host.Modules.GetModulesByType<LoggingModule>();

            if (myModule.Count > 0)
            {
                myModule[0].Log(LoggingEvent.Severity.Trace, "Hello trace!");
                myModule[0].Log(LoggingEvent.Severity.Debug, "Hello debug!");
                myModule[0].Log(LoggingEvent.Severity.Information, "Hello information!");
                myModule[0].Log(LoggingEvent.Severity.Notice, "Hello notice!");
                myModule[0].Log(LoggingEvent.Severity.Warning, "Hello warning!");
                myModule[0].Log(LoggingEvent.Severity.Error, "Hello error!");
                myModule[0].Log(LoggingEvent.Severity.Critical, "Hello critical!");
                myModule[0].Log(LoggingEvent.Severity.Alert, "Hello alert!");
                myModule[0].Log(LoggingEvent.Severity.Emergency, "Hello emergency!");
            }

            var lastLine = new LastLineEvent();
            host.Handle(lastLine);
            Console.WriteLine($"Last line: {lastLine.Output.Log}");

            var readLog = new ReadLoggingFileEvent()
            {
                Input = new ReadLoggingFileEventInput()
                {
                    Lines = 1,
                    ReadMode = Events.Enums.ReadMode.Tail
                }
            };

            host.Handle(readLog);
            Console.WriteLine($"Read log: {readLog.Output.Log}");

            var searchLog = new SearchLoggingFileEvent()
            {
                Input = new SearchLoggingFileEventInput()
                {
                    MaxLines = 1,
                    Query = "debug"
                }
            };
            
            host.Handle(searchLog);
            Console.WriteLine($"Search log: {searchLog.Output.Log}");

            BlockingHandle.WaitOne();
        }
    }
}
