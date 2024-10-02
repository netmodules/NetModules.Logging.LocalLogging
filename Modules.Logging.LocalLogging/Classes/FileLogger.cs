using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Modules.Logging.LocalLogging.Events.Enums;
using NetModules;
using NetModules.Events;
using NetTools.Logging;

namespace Modules.Logging.LocalLogging.Classes
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
        Timer LoggingThread;
        Module Module;
        Queue<string> Queue;
        string LastLine;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="logFileSize">The size of a log file in megabytes before rotating the log file.</param>
        /// <param name="logRotationFileCount">The number of log rotation files to keep.</param>
        public FileLogger(Module module, ushort logFileSize, ushort logRotationFileCount, LoggingEvent.Severity maxLoggingLevel)
        {
            Module = module;
            Queue = new Queue<string>();
            LogFilePath = Path.Combine(module.WorkingDirectory.LocalPath, "logs", "error.log");
            SetMaxLoggingLevel(maxLoggingLevel);

            var logDir = Path.GetDirectoryName(LogFilePath);

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            Rotate(LogFilePath);

            lock (Queue)
            {
                using (StreamWriter sw = new StreamWriter(LogFilePath, true))
                {
                    sw.WriteLine($"{LoggingHelpers.GetDateString()}: {Module.ModuleAttributes.Name}");
                    sw.WriteLine($">Initializing error log file for {Module.Host.ApplicationName}");
                    sw.WriteLine($">Working directory: {Module.WorkingDirectory.LocalPath}");
                    sw.WriteLine();
                }
            }

            // Kilobytes * megabytes * logfileSize.
            LogFileSize = 1024 * 1024 * logFileSize;
            LogFileCount = logRotationFileCount;

            // Create and start the log file writing thread.
            LoggingThread = new Timer((state) =>
            {
                Write(LogFilePath);
                Rotate(LogFilePath);
            }, null, 0, LogFileWriteDelay);
        }

        // Destructor...
        ~FileLogger()
        {
            Write(LogFilePath);
            //Rotate(LogFilePath);
        }

        internal string GetLastLine()
        {
            return LastLine;
        }

        internal void SetMaxLoggingLevel(LoggingEvent.Severity maxLoggingLevel)
        {
            MaxLoggingLevel = maxLoggingLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Error(params object[] args)
        {
            var line = $"{LoggingHelpers.GetDateString()}> ERROR: {LoggingHelpers.GetPrintableArgs(args)}";
            LastLine = line;

            Queue.Enqueue(line);
        }

        public override void Analytic(params object[] args)
        {
            var line = $"{LoggingHelpers.GetDateString()}> INFORMATION: {LoggingHelpers.GetPrintableArgs(args)}";
            LastLine = line;


            if (MaxLoggingLevel == LoggingEvent.Severity.Information)
            {
                Queue.Enqueue(line);
            }
        }

        public override void Debug(params object[] args)
        {
            var line = $"{LoggingHelpers.GetDateString()}> DEBUG: {LoggingHelpers.GetPrintableArgs(args)}";
            LastLine = line;

            if (MaxLoggingLevel == LoggingEvent.Severity.Information
                || MaxLoggingLevel == LoggingEvent.Severity.Debug)
            {
                Queue.Enqueue(line);
            }
        }

        public override void Information(params object[] args)
        {
            var line = $"{LoggingHelpers.GetDateString()}> WARNING: {LoggingHelpers.GetPrintableArgs(args)}";
            LastLine = line;

            if (MaxLoggingLevel == LoggingEvent.Severity.Information
                || MaxLoggingLevel == LoggingEvent.Severity.Debug
                || MaxLoggingLevel == LoggingEvent.Severity.Warning)
            {
                Queue.Enqueue(line);
            }
        }

        
        /// <summary>
        /// 
        /// </summary>
        internal void Write(string path)
        {
            try
            {
                // Put a lock on the queue to stop file reads while writing log file...
                lock (Queue)
                {
                    if (Queue.Count > 0)
                    {
                        using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                        {
                            while (Queue.Count > 0)
                            {
                                var current = Queue.Dequeue();

                                if (!string.IsNullOrWhiteSpace(current))
                                {
                                    sw.WriteLine(current);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                Module.Log(LoggingEvent.Severity.Debug, $"Unable to write to log file at {LogFilePath}");
            }
        }


        internal string Read(ushort lines, ulong skipLines = 0, ReadMode mode = ReadMode.Tail)
        {
            if (mode == ReadMode.Tail)
            {
                return ReadUp(lines, skipLines);
            }

            return ReadDown(lines, skipLines);
        }


        internal string ReadDown(ushort lines, ulong skipLines = 0)
        {
            if (lines == 0)
            {
                lines = 1;
            }

            // Multiply by 2 to account for newline spacing...
            lines = (ushort)Math.Clamp(lines * 2, 2, ushort.MaxValue);
            skipLines = skipLines * 2;

            // Put a lock on the queue to stop file writes while reading log file...
            lock (Queue)
            {
                using (FileStream stream = new FileStream(LogFilePath, FileMode.Open))
                {
                    ulong skipCount = 0;
                    long skipPosition = 0;
                    int newLines = 0;
                    
                    stream.Seek(0, SeekOrigin.Begin);

                    while (newLines < lines + 1 && stream.Position != stream.Length)
                    {
                        var currentByte = stream.ReadByte();

                        // look for \n (newline)...
                        if (currentByte == '\n')
                        {
                            if (stream.ReadByte() == '>')
                            {
                                continue;
                            }

                            if (skipCount < skipLines)
                            {
                                skipCount++;
                                skipPosition = stream.Position;
                            }
                            else
                            {
                                newLines++;
                            }
                        }
                    }

                    stream.Seek(-1, SeekOrigin.Current);

                    byte[] buffer = new byte[stream.Position - skipPosition];

                    stream.Seek(skipPosition, SeekOrigin.Begin);
                    stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer).Trim();
                }
            }
        }


        internal string ReadUp(ushort lines, ulong skipLines = 0)
        {
            if (lines == 0)
            {
                lines = 1;
            }

            // Multiply by 2 to account for newline spacing...
            lines = (ushort)Math.Clamp(lines * 2, 2, ushort.MaxValue);
            skipLines = skipLines * 2;

            // Put a lock on the queue to stop file writes while reading log file...
            lock (Queue)
            {
                using (FileStream stream = new FileStream(LogFilePath, FileMode.Open))
                {
                    ulong skipCount = 0;
                    long skipPosition = 0;
                    int newLines = 0;
                    int lastByte = 0;

                    stream.Seek(0, SeekOrigin.End);
                    
                    while (newLines < lines + 1 && stream.Position != 0)
                    {
                        stream.Seek(-1, SeekOrigin.Current);
                        var currentByte = stream.ReadByte();

                        // look for \n (newline)...
                        if (currentByte == '\n')
                        {
                            if (lastByte != '>' || stream.Position == 1)
                            {
                                if (skipCount < skipLines)
                                {
                                    skipCount++;
                                    skipPosition = stream.Position;
                                }
                                else
                                {
                                    newLines++;
                                }
                            }
                        }

                        lastByte = currentByte;
                        stream.Seek(-1, SeekOrigin.Current);
                    }

                    byte[] buffer = new byte[skipPosition > 0 ? stream.Length - (stream.Length - skipPosition) - stream.Position : stream.Length - stream.Position];
                    stream.Read(buffer,0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer).Trim();
                }
            }
        }


        internal string Search(string query, ushort maxLines = 0)
        {
            if (maxLines == 0)
            {
                maxLines = 1;
            }

            List<string> lines = new List<string>();

            // Put a lock on the queue to stop file writes while reading log file...
            lock (Queue)
            {
                using (FileStream stream = new FileStream(LogFilePath, FileMode.Open))
                {
                    //long lineCount = 0;
                    long lastLinePosition = stream.Length;
                    long linePosition = stream.Length;
                    int lastByte = 0;

                    stream.Seek(0, SeekOrigin.End);

                    while (lines.Count < maxLines && stream.Position != 0)
                    {
                        stream.Seek(-1, SeekOrigin.Current);
                        var currentByte = stream.ReadByte();

                        // look for \n (newline)...
                        if (currentByte == '\n')
                        {
                            if (lastByte != '>' || stream.Position == 1)
                            {
                                lastLinePosition = linePosition;
                                linePosition = stream.Position;

                                byte[] buffer = new byte[lastLinePosition - linePosition];
                                stream.Read(buffer, 0, buffer.Length);

                                var s = Encoding.UTF8.GetString(buffer).Trim();

                                if (s.Contains(query, StringComparison.OrdinalIgnoreCase))
                                {
                                    lines.Add(s);
                                }

                                stream.Position = linePosition;
                            }
                        }

                        lastByte = currentByte;
                        stream.Seek(-1, SeekOrigin.Current);
                    }

                    return string.Join('\n', lines);
                }
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

            // Put a lock on the queue to stop file read/write while moving log file rotation...
            lock (Queue)
            {
                try
                {
                    File.Move(path, rotatedPath);
                }
                catch { }
            }
            
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
