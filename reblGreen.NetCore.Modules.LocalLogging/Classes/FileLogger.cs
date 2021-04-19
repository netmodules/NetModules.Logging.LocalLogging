using reblGreen.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace reblGreen.NetCore.Modules.LocalLogging.Classes
{
    internal class FileLogger : BaseLogger
    {
        /// <summary>
        /// The delay in milliseconds between file writes. Log messages are queued and written in bursts
        /// to reduce system overhead.
        /// </summary>
        const ushort LogFileWriteDelay = 5000; // 5 seconds

        Events.LoggingEvent.Severity MaxLoggingLevel;
        string LogFilePath;
        int LogFileSize;
        ushort LogFileCount;
        Thread LoggingThread;
        Module Module;
        Queue<string> Queue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="logFileSize">The size of a log file in megabytes before rotating the log file.</param>
        /// <param name="logRotationFileCount">The number of log rotation files to keep.</param>
        public FileLogger(Module module, ushort logFileSize, ushort logRotationFileCount, Events.LoggingEvent.Severity maxLoggingLevel)
        {
            Module = module;
            MaxLoggingLevel = maxLoggingLevel;
            Queue = new Queue<string>();
            LogFilePath = Path.Combine(module.WorkingDirectory.LocalPath, $"logs{Path.DirectorySeparatorChar}error.log");

            var logDir = Path.GetDirectoryName(LogFilePath);

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            lock (Queue)
            {
                using (StreamWriter sw = new StreamWriter(LogFilePath, true))
                {
                    sw.WriteLine($"Initializing error log file for {Module.Host.ApplicationName}");
                    sw.WriteLine($"Working directory: {Module.WorkingDirectory.LocalPath}");
                }
            }

            // Kilobytes * megabytes * logfileSize.
            LogFileSize = 1024 * 1024 * logFileSize;
            LogFileCount = logRotationFileCount;

            // Create and start the log file writing thread.
            LoggingThread = new Thread(() =>
            {
                while (true)
                {
                    WriteFile(LogFilePath);
                    Rotate(LogFilePath);
                    Thread.Sleep(LogFileWriteDelay);
                }
            })
            {
                IsBackground = true,
                Name = "LoggingThread",
                Priority = ThreadPriority.Lowest
            };

            LoggingThread.Start();
        }

        // Destructor...
        ~FileLogger()
        {
            WriteFile(LogFilePath);
            Rotate(LogFilePath);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Error(params object[] args)
        {
            Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
        }

        public override void Analytic(params object[] args)
        {
            if (MaxLoggingLevel == Events.LoggingEvent.Severity.Analytics)
            {
                Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
            }
        }

        public override void Debug(params object[] args)
        {
            if (MaxLoggingLevel == Events.LoggingEvent.Severity.Analytics
                || MaxLoggingLevel == Events.LoggingEvent.Severity.Debug)
            {
                Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
            }
        }

        public override void Information(params object[] args)
        {
            if (MaxLoggingLevel == Events.LoggingEvent.Severity.Analytics
                || MaxLoggingLevel == Events.LoggingEvent.Severity.Debug
                || MaxLoggingLevel == Events.LoggingEvent.Severity.Warning)
            {
                Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        internal void WriteFile(string path)
        {
            try
            {
                lock (Queue)
                {
                    using (StreamWriter sw = new StreamWriter(path, true))
                    {
                        while (Queue.Count > 0)
                        {
                            sw.WriteLine(Queue.Dequeue());
                        }
                    }
                }
            }
            catch
            {
                Module.Log(Events.LoggingEvent.Severity.Debug, $"Unable to write to log file at {LogFilePath}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void Rotate(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var fileInfo = new FileInfo(path);
            if (fileInfo.Length < LogFileSize)
            {
                return;
            }

            var folderPath = Path.GetDirectoryName(path);
            var logFolderContent = new DirectoryInfo(folderPath).GetFileSystemInfos();

            var rotated = logFolderContent.Where(x => !x.Extension.Equals(".log", StringComparison.OrdinalIgnoreCase));

            var rotatedPath = path.Replace(".log", $".log.{DateTime.UtcNow.Ticks}", StringComparison.OrdinalIgnoreCase);

            try
            {
                File.Move(path, rotatedPath);
            }
            catch { }

            
            if (rotated.Count() < LogFileCount - 1)
            {
                return;
            }

            var oldest = rotated.OrderBy(x => x.CreationTime).Take(rotated.Count() - (LogFileCount - 1));

            try
            {
                foreach (var f in oldest)
                {
                    File.Delete(f.FullName);
                }
            }
            catch { }
        }
    }
}
