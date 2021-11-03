using reblGreen;
using reblGreen.NetCore.Modules;
using reblGreen.NetCore.Modules.Events;
using System;
using System.Threading;

namespace Modules.LocalLogging.TestApplication
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

                while (true)
                {
                    myModule[0].Log(LoggingEvent.Severity.Error, new Exception("This is an error!"), "Hello world!");
                    Thread.Sleep(1000);
                }
            }

            BlockingHandle.WaitOne();
        }
    }
}
