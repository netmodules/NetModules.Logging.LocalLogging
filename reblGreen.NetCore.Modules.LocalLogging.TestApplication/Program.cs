using System;
using System.Threading;

namespace reblGreen.NetCore.Modules.LocalLogging.TestApplication
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
                myModule[0].Log(Events.LoggingEvent.Severity.Debug, "Hello debug!");
                myModule[0].Log(Events.LoggingEvent.Severity.Warning, "Hello warning!");
                myModule[0].Log(Events.LoggingEvent.Severity.Error, "Hello error!");
                myModule[0].Log(Events.LoggingEvent.Severity.Analytics, "Hello analytics!");

                while (true)
                {
                    myModule[0].Log(Events.LoggingEvent.Severity.Error, new Exception("This is an error!"), "Hello world!");
                    Thread.Sleep(1000);
                }
            }

            BlockingHandle.WaitOne();
        }
    }
}
