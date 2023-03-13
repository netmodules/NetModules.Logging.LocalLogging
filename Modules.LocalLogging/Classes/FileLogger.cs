using reblGreen;
using NetModules;
using reblGreen.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NetModules.Events;

namespace Modules.LocalLogging.Classes
{
    internal class FileLogger : BaseLogger
    {
        /// <summary>
        /// The delay in milliseconds between file writes. Log messages are queued and written in bursts
        /// to reduce system overhead.
        /// </summary>
        const ushort LogFileWriteDelay = 5000; // 5 seconds

        LoggingEvent.Severity MaxLoggingLevel;
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
        public FileLogger(Module module, ushort logFileSize, ushort logRotationFileCount, LoggingEvent.Severity maxLoggingLevel)
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
            if (MaxLoggingLevel == LoggingEvent.Severity.Information)
            {
                Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
            }
        }

        public override void Debug(params object[] args)
        {
            if (MaxLoggingLevel == LoggingEvent.Severity.Information
                || MaxLoggingLevel == LoggingEvent.Severity.Debug)
            {
                Queue.Enqueue($"{LoggingHelpers.GetDateString()} {LoggingHelpers.GetPrintableArgs(args)}");
            }
        }

        public override void Information(params object[] args)
        {
            if (MaxLoggingLevel == LoggingEvent.Severity.Information
                || MaxLoggingLevel == LoggingEvent.Severity.Debug
                || MaxLoggingLevel == LoggingEvent.Severity.Warning)
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
                    using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
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
                Module.Log(LoggingEvent.Severity.Debug, $"Unable to write to log file at {LogFilePath}");
            }
        }


        internal string ReadFile(ushort lines)
        {
            if (lines == 0)
            {
                lines = 1;
            }

            var charsize = Encoding.UTF8.GetByteCount("\n");
            var buffer = Encoding.UTF8.GetBytes("\n");
            var count = 0;

            using (FileStream stream = new FileStream(LogFilePath, FileMode.Open))
            {
                var endpos = stream.Length / charsize;

                for (var pos = charsize; pos < endpos; pos += charsize)
                {
                    stream.Seek(-pos, SeekOrigin.End);
                    stream.Read(buffer, 0, buffer.Length);

                    if (Encoding.UTF8.GetString(buffer) == "\n")
                    {
                        if (count >= lines)
                        {
                            buffer = new byte[stream.Length - stream.Position];
                            stream.Read(buffer, 0, buffer.Length);
                            return Encoding.UTF8.GetString(buffer);
                        }

                        count++;
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
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
