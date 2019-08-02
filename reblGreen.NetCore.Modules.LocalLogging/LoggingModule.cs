using System;
using reblGreen.NetCore.Modules.Events;
using reblGreen.NetCore.Modules.Interfaces;
using reblGreen.NetCore.Modules.LocalLogging.Classes;

namespace reblGreen.NetCore.Modules.LocalLogging
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    [Module(
        LoadPriority = short.MaxValue - 1, HandlePriority = short.MinValue,
        Description = ""
    )]
    public class LoggingModule : Module
    {
        LoggingHandler LoggingHandler;

        public override bool CanHandle(IEvent e)
        {
            if (e is LoggingEvent)
            {
                return true;
            }

            return false;
        }

        public override void Handle(IEvent e)
        {
            if (e is LoggingEvent @event)
            {
                LoggingHandler.LogEvent(@event);
            }
        }

        public override void OnLoading()
        {
            var logFileSize = (ushort)GetSetting("logFileSize", 100);
            var logRotationFileCount = (ushort)GetSetting("logRotationFileCount", 10);
            LoggingHandler = new LoggingHandler(this, logFileSize, logRotationFileCount);
            base.OnLoading();
        }


        public override void OnLoaded()
        {
            base.OnLoaded();
        }


        public override void OnUnloading()
        {
            base.OnUnloading();
        }
    }
}
