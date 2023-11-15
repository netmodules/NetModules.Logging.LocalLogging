using System;
using System.Threading;
using NetModules;
using NetModules.Events;
using Modules.Logging.LocalLogging.Events;

namespace Modules.Logging.LocalLogging.TestApplication
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

            var myModule = host.Modules.GetModulesByType<LoggingModule>();

            if (myModule.Count > 0)
            {
                myModule[0].Log(LoggingEvent.Severity.Debug, "Hello debug!");
                myModule[0].Log(LoggingEvent.Severity.Warning, "Hello warning!");
                myModule[0].Log(LoggingEvent.Severity.Error, "Hello error!");
                myModule[0].Log(LoggingEvent.Severity.Information, "Hello information!");
                myModule[0].Log(LoggingEvent.Severity.Debug, "Hello debug!");
                myModule[0].Log(LoggingEvent.Severity.Warning, "Hello warning!");
                myModule[0].Log(LoggingEvent.Severity.Error, "Hello error!");
                myModule[0].Log(LoggingEvent.Severity.Information, "Hello information!");
                myModule[0].Log(LoggingEvent.Severity.Debug, "Hello debug!");
                myModule[0].Log(LoggingEvent.Severity.Warning, "Hello warning!");
                myModule[0].Log(LoggingEvent.Severity.Error, "Hello error!");
                myModule[0].Log(LoggingEvent.Severity.Information, "Hello information!");
                myModule[0].Log(LoggingEvent.Severity.Debug, "Hello debug!");
                myModule[0].Log(LoggingEvent.Severity.Warning, "Hello warning!");
                myModule[0].Log(LoggingEvent.Severity.Error, "Hello error!");
                myModule[0].Log(LoggingEvent.Severity.Information, "Hello information!");

                while (true)
                {
                    myModule[0].Log(LoggingEvent.Severity.Error, new Exception("This is an error!"), "Hello world!");
                    Thread.Sleep(1000);

                    var read = new ReadLoggingFileEvent
                    {
                        Input = new ReadLoggingFileEventInput
                        {
                            Lines = 10,
                        }
                    };

                    host.Handle(read);

                    var read2 = new ReadLoggingFileEvent
                    {
                        Input = new ReadLoggingFileEventInput
                        {
                            Lines = 10,
                            SkipLines = 9,
                        }
                    };

                    host.Handle(read2);


                    var search = new SearchLoggingFileEvent
                    {
                        Input = new SearchLoggingFileEventInput
                        {
                            Query = "this",
                            MaxLines = 2,
                        }
                    };

                    host.Handle(search);


                    var last = new LastLineEvent();
                    host.Handle(last);
                }
            }

            BlockingHandle.WaitOne();
        }
    }
}
