using reblGreen.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace reblGreen.NetCore.Modules.LocalLogging.Classes
{
    internal class ErrorFileLogger : BaseLogger
    {
        /// <summary>
        /// The delay in milliseconds between file writes. Log messages are queued and written in bursts
        /// to reduce system overhead.
        /// </summary>
        const ushort LogFileWriteDelay = 5000;

        string LogFilePath;
        int LogFileSize;
        ushort LogFileCount;
        Thread LoggingThread;

        Queue<string> Queue;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="modulePath">The module path can be found in Module.WorkingDirectory</param>
        /// <param name="logFileSize">The size of a log file in megabytes before rotating the log file.</param>
        /// <param name="logRotationFileCount">The number of log rotation files to keep.</param>
        public ErrorFileLogger(string modulePath, ushort logFileSize, ushort logRotationFileCount)
        {
            Queue = new Queue<string>();
            LogFilePath = Path.Combine(modulePath, $"logs{Path.DirectorySeparatorChar}error.log");

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


        /// <summary>
        /// 
        /// </summary>
        public override void Error(params object[] args)
        {
            Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
        }

        
        /// <summary>
        /// 
        /// </summary>
        internal void WriteFile(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    while (Queue.Count > 0)
                    {
                        sw.WriteLine(Queue.Dequeue());
                    }
                }
            }
            catch { }
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
