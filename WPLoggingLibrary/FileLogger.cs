using System.IO.Compression;

namespace WPLoggingLibrary
{
    public enum LogLevel
    {
        Debug = -1,
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public interface ILogger
    {
        void Log(LogLevel level, string message);
    }

    public sealed class FileLogger : ILogger, IDisposable
    {
        private static readonly Lazy<FileLogger> _instance = new(() => new FileLogger());
        public static FileLogger Instance => _instance.Value;

        private readonly string _logDirectory;
        private readonly string _logFileName;
        private readonly string _logFilePath;
        private readonly object _lockObject = new();
        private StreamWriter? _streamWriter;
        private readonly LogLevel _minimumLogLevel;
        private readonly Dictionary<LogLevel, Action<string>> _loggingHooks;
        private readonly Queue<(LogLevel level, string message)> _messageQueue;
        private readonly Thread _logThread;
        private bool _isRunning = true;

        private FileLogger()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            _logFileName = DateTime.Now.ToString("dd.MM.yyyy") + ".log";
            _logFilePath = Path.Combine(_logDirectory, _logFileName);
            _minimumLogLevel = LogLevel.Debug;

            EnsureLogDirectoryExists();

            InitializeStreamWriter();

            _loggingHooks = new Dictionary<LogLevel, Action<string>>
            {
                { LogLevel.Error, message => WriteLogEntry(LogLevel.Error, message) }
            };

            _messageQueue = new Queue<(LogLevel, string)>();
            _logThread = new Thread(WriteMessagesFromQueue);
            _logThread.Start();
        }

        private void EnsureLogDirectoryExists()
        {
            Directory.CreateDirectory(_logDirectory);
        }

        private void InitializeStreamWriter()
        {
            lock (_lockObject)
            {
                try
                {
                    _streamWriter = File.AppendText(_logFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing StreamWriter: {ex.Message}");
                    throw;
                }
            }
        }

        private void WriteMessagesFromQueue()
        {
            while (_isRunning)
            {
                if (_messageQueue.Count > 0)
                {
                    (LogLevel level, string message) = _messageQueue.Dequeue();
                    WriteLogEntry(level, message);
                }
                else
                {
                    Thread.Sleep(100); // Wait for messages in queue
                }
            }
        }

        public void Log(LogLevel level, string message)
        {
            if (level <= _minimumLogLevel)
            {
                WriteLogEntry(level, message);
            }
            else if (_loggingHooks.TryGetValue(level, out Action<string>? value))
            {
                value?.Invoke(message);
            }
            else
            {
                _messageQueue.Enqueue((level, message));
            }
        }

        private void WriteLogEntry(LogLevel level, string message)
        {
            string logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} :: LogType.{level} :: {message}";

            try
            {
                lock (_lockObject)
                {
                    RotateLogFileIfNeeded();
                    if (_streamWriter != null)
                    {
                        _streamWriter.WriteLine(logEntry);
                        _streamWriter.Flush();
                    }
                    else
                    {
                        throw new Exception("The stream writer was not ready!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        private void RotateLogFileIfNeeded()
        {
            if (!File.Exists(_logFilePath))
            {
                return;
            }

            DateTime currentDate = DateTime.UtcNow.Date;
            DateTime lastWriteTime = File.GetLastWriteTimeUtc(_logFilePath).Date;

            if (lastWriteTime < currentDate)
            {
                string zipFilePath = Path.Combine(_logDirectory, "log.zip");

                if (!File.Exists(zipFilePath))
                {
                    using FileStream fileStream = new(zipFilePath, FileMode.Create);
                    using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Create);
                    zipArchive.CreateEntryFromFile(_logFilePath, _logFileName);
                }
                else
                {
                    using FileStream fileStream = new(zipFilePath, FileMode.Open);
                    using ZipArchive zipArchive = new(fileStream, ZipArchiveMode.Update);
                    zipArchive.CreateEntryFromFile(_logFilePath, _logFileName);
                }

                File.Delete(_logFilePath);
                InitializeStreamWriter();

                // Behalte die letzten 3 Dateien im Archiv, lösche Dateien im Archiv, die älter als 3 Monate sind
                using ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update);
                var entries = archive.Entries
                    .OrderByDescending(e => e.LastWriteTime)
                    .ToList();

                var filesToDelete = entries.Skip(3).Where(e => e.LastWriteTime.DateTime < currentDate.AddMonths(-3)).ToList();
                foreach (var fileEntry in filesToDelete)
                {
                    fileEntry.Delete();
                }
            }
        }

        public void Dispose()
        {
            _isRunning = false;
            _logThread.Join();
            lock (_lockObject)
            {
                _streamWriter?.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
