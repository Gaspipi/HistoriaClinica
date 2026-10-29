using System.Globalization;
using System.Text;
using HistoriaClinica.Core.Data;
using Microsoft.Extensions.Logging;

namespace HistoriaClinica.Core.Logging;

/// <summary>
/// Proveedor de logging que escribe a un archivo de texto con rotación diaria.
/// </summary>
public class FileLoggerProvider : ILoggerProvider, IDisposable
{
    private readonly string _logDirectory;
    private readonly CancellationToken _cancellationToken;
    private readonly object _sync = new();
    private StreamWriter? _writer;
    private string? _currentDate;
    private bool _disposed;

    public FileLoggerProvider(string logDirectory, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(logDirectory);

        var directory = logDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (!string.Equals(Path.GetFileName(directory), "Logs", StringComparison.OrdinalIgnoreCase))
        {
            directory = Path.Combine(directory, "Logs");
        }

        _logDirectory = directory;
        _cancellationToken = cancellationToken;
    }

    public FileLoggerProvider(IDatabasePathProvider pathProvider, CancellationToken cancellationToken = default)
        : this(Path.GetDirectoryName(pathProvider.DatabasePath) ?? ".", cancellationToken)
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return new FileLogger(this, categoryName);
    }

    internal void WriteEntry(LogLevel logLevel, string categoryName, string message, Exception? exception)
    {
        if (_disposed)
        {
            return;
        }

        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            if (_cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var now = DateTime.Now;
            var date = now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (_writer is null || !string.Equals(_currentDate, date, StringComparison.Ordinal))
            {
                _writer?.Dispose();
                Directory.CreateDirectory(_logDirectory);
                var filePath = Path.Combine(_logDirectory, $"historia-{date}.log");
                _writer = new StreamWriter(filePath, append: true, Encoding.UTF8)
                {
                    AutoFlush = true,
                };
                _currentDate = date;
            }

            var line = $"{now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {message}";
            _writer.WriteLine(line);

            if (exception is not null)
            {
                _writer.WriteLine(exception.ToString());
            }
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _writer?.Dispose();
            _writer = null;
        }
    }

    private sealed class FileLogger : ILogger
    {
        private readonly FileLoggerProvider _provider;
        private readonly string _categoryName;

        public FileLogger(FileLoggerProvider provider, string categoryName)
        {
            _provider = provider;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            _provider.WriteEntry(logLevel, _categoryName, message, exception);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
